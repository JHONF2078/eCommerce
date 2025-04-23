using Polly;

namespace OrdersService.BusinessLogicLayer.Policies;

public interface IProductsMicroservicePolicies
{
  IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy();
  IAsyncPolicy<HttpResponseMessage> GetBulkheadIsolationPolicy();
}
