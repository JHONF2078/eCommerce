using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ProductsService.BusinessLogicLayer.RabbitMQ
{
    public class RabbitMQPublisher: IRabbitMQPublisher, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IModel _channel;
        private readonly IConnection _connection;

        public RabbitMQPublisher(IConfiguration configuration)
        {
            _configuration = configuration;
            string hostName = _configuration["RABBITMQ_HOSTNAME"]!;
            string usertName = _configuration["RABBITMQ_USERNAME"]!;
            string password = _configuration["RABBITMQ_PASSWORD"]!;
            string port = _configuration["RABBITMQ_PORT"]!;
           
            ConnectionFactory connectionFactory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = usertName,                      
                Password = password,
                Port = Convert.ToInt32(port)
            };

            //se crea un objeto de tipo CONEXION
            //representa una conexion TPC /IP a un servidor RabbitMQ
            _connection = connectionFactory.CreateConnection();

            //se crea un objeto de tipo CANAL
            // esto usa el protocolo AMQP para comunicarse con el servidor RabbitMQ
            // este protocolo actua como una comunicacion real para poder cominicarse con rabbit
            //gracias a este channel podmeos trabajar con intercmbios, colas, publicar,
            //consumir mensajes
            _channel = _connection.CreateModel();
        }

        /// <summary>
        /// Dispose the RabbitMQ connection and channel.
        /// </summary>
        public void Dispose()
        {
            //cierra o elimina las conexiones y canales
            //al final de la vida util del servicio
            //no necesitamos llamar a Dispose() manualmente
            //ASP.NET CORE lo hace automaticamente
            _channel.Dispose();
            _connection.Dispose();
        }

        public void Publish<T>(Dictionary<string, object> headers, T message)
        {
            //los mensajes se publican en el exhange en el intercambio
            //los objetos no pueden ser publicados directamente en el exchange
            //solo se puede subir informacion binaria como tipos byte
            //primero se debe convertir el objeto a un tipo json y luego convertirlo 
            //a un arreglo de bytes

            //alternativamente puedes usar Newtonsoft
            //toma el objeto mensjae y dame un string
            string messageJson = JsonSerializer.Serialize(message);

            //ahora convierto este string a un arreglo de bytes
            //Encoding.UTF8 es un tipo de codificacion que convierte el string
            byte[] messageBodyInBytes = Encoding.UTF8.GetBytes(messageJson);

            //ahora creamos el exchange (intercambio)
            string exchangeName = _configuration["RABBITMQ_PRODUCTS_EXCHANGE"]!;

           // Console.WriteLine($"📤 Publicando en exchange: {exchangeName}, con routingKey: {routingKey}");


            _channel.ExchangeDeclare(
                exchange: exchangeName,
                type: ExchangeType.Headers,
                durable: true
              );

            var bascicProperties = _channel.CreateBasicProperties();
            bascicProperties.Headers = headers;

            //ahora posteamos el mensaje al exchange
            //routingKey es la clave de enrutamiento oevento por ejemplo
            //updating, deleting, adding o otra accion especifica como
            //order, returning, canceling, etc
            //nunca se postea directamente en la cola, siempre es en el chanel
            //Nota: las colas no se definen en el productor, solo en el consumidor
            _channel.BasicPublish(
                exchange: exchangeName,
                routingKey: string.Empty,
                basicProperties: bascicProperties,
                body: messageBodyInBytes
             ); 
        }
    }   
}
