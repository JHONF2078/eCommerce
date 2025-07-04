﻿using Microsoft.Extensions.Hosting;

namespace OrdersService.BusinessLogicLayer.RabbitMQ;

public class RabbitMQProductNameUpdateHostedService : IHostedService
{
  private readonly IRabbitMQProductNameUpdateConsumer _productNameUpdateConsumer;

  public RabbitMQProductNameUpdateHostedService(IRabbitMQProductNameUpdateConsumer consumer)
  {
    //invocamos al metodo consume
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