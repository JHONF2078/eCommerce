using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace OrdersService.BusinessLogicLayer.Policies
{
    public class UsersMicroservicePolicies : IUsersMicroservicePolices
    {

        private readonly ILogger<UsersMicroservicePolicies> _logger;

        public UsersMicroservicePolicies(ILogger<UsersMicroservicePolicies> logger)
        {
            _logger = logger;
        }

        public IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            AsyncCircuitBreakerPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(
              r => !r.IsSuccessStatusCode
            )
           .CircuitBreakerAsync(
               handledEventsAllowedBeforeBreaking: 3, //Number of fallas consecutivas
               durationOfBreak: TimeSpan.FromMinutes(2), //tiempo durante el cual estara abierto el breaker (tiempo cerrado a peticiones)
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

        public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            AsyncRetryPolicy<HttpResponseMessage> policy = Policy.HandleResult<HttpResponseMessage>(
                r => !r.IsSuccessStatusCode
              )
             .WaitAndRetryAsync(
                 retryCount: 5, //Number of retries
                 sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(
                    Math.Pow(2, retryAttempt)
                  ), //Exponential backoff
                 onRetry: (outcome, timespan, retryAttempt, context) =>
                 {
                    _logger.LogInformation($"Retry {retryAttempt} after {timespan.TotalSeconds} seconds");
                 });

            return policy;
        }
    }
}
