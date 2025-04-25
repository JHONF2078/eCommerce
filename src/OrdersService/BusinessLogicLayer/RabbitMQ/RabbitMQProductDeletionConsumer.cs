using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace OrdersService.BusinessLogicLayer.RabbitMQ;

public class RabbitMQProductDeletionConsumer : IDisposable, IRabbitMQProductDeletionConsumer
{
    private readonly IConfiguration _configuration;
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly ILogger<RabbitMQProductDeletionConsumer> _logger;
    private readonly IDistributedCache _cache;

      public RabbitMQProductDeletionConsumer(
          IConfiguration configuration, 
          ILogger<RabbitMQProductDeletionConsumer> logger,
          IDistributedCache cache)
      {
         _configuration = configuration;

         string hostName = _configuration["RabbitMQ_HostName"]!;
         string userName = _configuration["RabbitMQ_UserName"]!;
         string password = _configuration["RabbitMQ_Password"]!;
         string port = _configuration["RabbitMQ_Port"]!;
         _logger = logger;


         ConnectionFactory connectionFactory = new ConnectionFactory()
         {
            HostName = hostName,
            UserName = userName,
            Password = password,
            Port = Convert.ToInt32(port)
         };
         _connection = connectionFactory.CreateConnection();

        _channel = _connection.CreateModel();
        _cache = cache;
      }


      public void Consume()
      {

        var  headers = new Dictionary<string, object>()
        {
            {"x-match", "all" }, //define si als claves deben coincidir complentamete o al menos uno (all, any)
            { "event", "product.delete" },
            { "RowCount", 1 }
        };


        string routingKey = "product.#";
        string queueName = "orders.product.delete.queue";

        //Create exchange
        string exchangeName = _configuration["RABBITMQ_PRODUCTS_EXCHANGE"]!;
        _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Headers, durable: true);

        //Create message queue
        _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null); //x-message-ttl | x-max-length | x-expired 

        //Bind the message to exchange
        _channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: string.Empty, arguments: headers);


        EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (sender, args) =>
        {
          byte[] body = args.Body.ToArray();
          string message = Encoding.UTF8.GetString(body);

          if (message != null)
          {
            ProductDeletionMessage? productDeletionMessage = JsonSerializer.Deserialize<ProductDeletionMessage>(message);

            if (productDeletionMessage != null)
            {
              _logger.LogInformation($"Product deleted: {productDeletionMessage.ProductID}, Product name: {productDeletionMessage.ProductName}");
               await HandleProductDeletion(productDeletionMessage.ProductID);
            }
          }
        };

        _channel.BasicConsume(queue: queueName, consumer: consumer, autoAck: true);
      }

    private async Task HandleProductDeletion(Guid productID)
    {
        string cacheKeyToWrite = $"product:{productID}";

        await _cache.RemoveAsync(cacheKeyToWrite);
    }

    public void Dispose()
      {
        _channel.Dispose();
        _connection.Dispose();
      }
}
