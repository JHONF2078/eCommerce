using Polly;

namespace OrdersService.BusinessLogicLayer.Policies
{
    public interface IPollyPolicies
    {
        //IAsyncPolicy represtan cualquier tipo de politica asincrona
        IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount);
        IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(
            int handledEventsAllowedBeforeBreaking,
            TimeSpan durationOfBreakInMinutes);
        IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(TimeSpan timeOut);       
    }
}
