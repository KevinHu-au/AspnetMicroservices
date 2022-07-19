using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polly;
using Polly.Extensions.Http;
using Serilog;

namespace Shopping.Aggregator.Extensions
{
    public static class PolicyHelper
    {
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(
                        retryCount: 5,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        onRetry: (result, retryCount, context) =>
                        {
                            Log.Error($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {result.Exception}.");
                        });
        }

        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30)
                );
        }
    }
}