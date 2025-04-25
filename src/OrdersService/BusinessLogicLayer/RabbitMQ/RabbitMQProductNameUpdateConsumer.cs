using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.Text.Json;

namespace OrdersService.BusinessLogicLayer.RabbitMQ;

public class RabbitMQProductNameUpdateConsumer : IDisposable, IRabbitMQProductNameUpdateConsumer
{
  private readonly IConfiguration _configuration;
  private readonly IModel _channel;
  private readonly IConnection _connection;
  private readonly ILogger<RabbitMQProductNameUpdateConsumer> _logger;

    public RabbitMQProductNameUpdateConsumer(
        IConfiguration configuration, ILogger<RabbitMQProductNameUpdateConsumer> logger)
    { 
        _logger = logger;
        _configuration = configuration;
        string hostName = _configuration["RABBITMQ_HOSTNAME"]!;
        string userName = _configuration["RABBITMQ_USERNAME"]!;
        string password = _configuration["RABBITMQ_PASSWORD"]!;
        string port = _configuration["RABBITMQ_PORT"]!;

        _logger.LogInformation($"Connecting to RabbitMQ at {hostName}:{port} with user {userName} and password {password}");

        ConnectionFactory connectionFactory = new ConnectionFactory
        {
            HostName = hostName,
            UserName = userName,
            Password = password,
            Port = Convert.ToInt32(port)
        };
        
        int maxRetries = 10;
        int retryDelayMilliseconds = 3000;
        int attempt = 0;

        while (true)
        {
            try
            {
                attempt++;
                _logger.LogInformation($"Intentando conectar a RabbitMQ (intento {attempt}/{maxRetries})...");

                _connection = connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();

                _logger.LogInformation("Conexión a RabbitMQ establecida correctamente.");
                break;
            }
            catch (BrokerUnreachableException ex)
            {
                if (attempt >= maxRetries)
                {
                    _logger.LogError(ex, "No se pudo conectar a RabbitMQ después de varios intentos.");
                    throw;
                }

                _logger.LogWarning($"Error al conectar con RabbitMQ. Reintentando en {retryDelayMilliseconds / 1000} segundos...");
                Thread.Sleep(retryDelayMilliseconds);
            }
        }
    }

    public void Consume()
    {
      string routingKey = "product.update.name";
        string queueName = "orders.product.update.name.queue";

        //Create exchange
        string exchangeName = _configuration["RABBITMQ_PRODUCTS_EXCHANGE"]!;
        _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct, durable: true);

        //Create message queue
        _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null); //x-message-ttl | x-max-length | x-expired 

        //Bind the message to exchange
        _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);


        EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (sender, args) =>
        {
            byte[] body = args.Body.ToArray();
            string message = Encoding.UTF8.GetString(body);

            if (message != null)
            {
                ProductNameUpdateMessage? productNameUpdateMessage = JsonSerializer.Deserialize<ProductNameUpdateMessage>(message);

                _logger.LogInformation($"Product name updated: {productNameUpdateMessage.ProductID}, New name: {productNameUpdateMessage.NewName}");
            }
        };

        _channel.BasicConsume(queue: queueName, consumer: consumer, autoAck: true);
    }


    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}
