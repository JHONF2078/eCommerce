using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;

namespace OrdersService.BusinessLogicLayer.Policies
{
    public class PollyPolicies : IPollyPolicies
    {

        private readonly ILogger<UsersMicroservicePolicies> _logger;

        public PollyPolicies(ILogger<UsersMicroservicePolicies> logger)
        {
            _logger = logger;
        }

        public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(
            int handledEventsAllowedBeforeBreaking,
            TimeSpan durationOfBreakInMinutes
        )
        {
            AsyncCircuitBreakerPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(
              r => !r.IsSuccessStatusCode
            )
           .CircuitBreakerAsync(
               handledEventsAllowedBeforeBreaking: handledEventsAllowedBeforeBreaking, //Number of fallas consecutivas
               durationOfBreak: durationOfBreakInMinutes, //tiempo durante el cual estara abierto el breaker (tiempo cerrado a peticiones)
               onBreak: (outcome, timespan) =>
               {
                   //circuit breaker OPEN
                   _logger.LogInformation($"Circuit Breaeker opened for {timespan.TotalMinutes} minutes due to consecutive 3 failures, The subsequent request will to be blocked");
               },
               onReset: () =>
               {
                   //circuit breaker  half-open
                   _logger.LogInformation("Circuit Breaker closed, the subsequent request will be allowed");
               }
               );

            return policy;
        }

        public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount)
        {
            AsyncRetryPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(
                r => !r.IsSuccessStatusCode
              )
             .WaitAndRetryAsync(
                 retryCount: retryCount, //Number of retries
                 sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(
                    Math.Pow(2, retryAttempt)
                  ), //Exponential backoff
                 onRetry: (outcome, timespan, retryAttempt, context) =>
                 {
                    _logger.LogInformation($"Retry {retryAttempt} after {timespan.TotalSeconds} seconds");
                 });

            return policy;
        }

        public IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(TimeSpan timeOut)
        {
            AsyncTimeoutPolicy<HttpResponseMessage> policy = Policy.TimeoutAsync<HttpResponseMessage>(timeOut);

            return policy;
        }      
    }
}
