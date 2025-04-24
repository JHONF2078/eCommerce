using Microsoft.Extensions.Configuration;

namespace ProductsService.BusinessLogicLayer.RabbitMQ
{

    /// <summary>
    /// Interface for RabbitMQ publisher.
    /// </summary>
    public interface IRabbitMQPublisher
    {

        //<T>: indica que el método es genérico, es decir, puede trabajar con
        //cualquier tipo de objeto(como Producto, Pedido, etc.).
        //routingKey: una cadena que indica la clave de enrutamiento que usará
        //RabbitMQ para dirigir el mensaje al destinatario correcto.
        //message: el contenido del mensaje, de tipo T
        void Publish<T>(string routingKey, T message);

    }
}
