using Polly;

namespace OrdersService.BusinessLogicLayer.Policies
{
    public interface IUsersMicroservicePolices
    {
        //IAsyncPolicy represtan cualquier tipo de politica asincrona
        IAsyncPolicy<HttpResponseMessage> GetRetryPolicy();
        IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy();
    }
}
