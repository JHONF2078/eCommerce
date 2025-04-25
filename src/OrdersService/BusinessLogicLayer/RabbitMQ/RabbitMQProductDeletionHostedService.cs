using Microsoft.Extensions.Hosting;

namespace OrdersService.BusinessLogicLayer.RabbitMQ;

public class RabbitMQProductDeletionHostedService : IHostedService
{
  private readonly IRabbitMQProductDeletionConsumer _productNameUpdateConsumer;

  public RabbitMQProductDeletionHostedService(IRabbitMQProductDeletionConsumer consumer)
  {
    _productNameUpdateConsumer = consumer;
  }


  public Task StartAsync(CancellationToken cancellationToken)
  {
    _productNameUpdateConsumer.Consume();

    return Task.CompletedTask;
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _productNameUpdateConsumer.Dispose();

    return Task.CompletedTask;
  }
}
