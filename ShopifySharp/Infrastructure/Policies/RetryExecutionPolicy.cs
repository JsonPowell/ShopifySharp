﻿using Flurl.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using ShopifySharp.Infrastructure;

namespace ShopifySharp
{
    /// <summary>
    /// See https://help.shopify.com/api/guides/api-call-limit
    /// </summary>
    public class RetryExecutionPolicy : IRequestExecutionPolicy
    {
        private static readonly TimeSpan RETRY_DELAY = TimeSpan.FromMilliseconds(500);

        public async Task<T> Run<T>(IFlurlClient baseRequest, JsonContent bodyContent, ExecuteRequestAsync<T> executeRequestAsync)
        {
            while (true)
            {
                var request = baseRequest.Clone();
                var content = bodyContent?.Clone();

                try
                {
                    var fullResult = await executeRequestAsync(request, content);

                    return fullResult.Result;
                }
                catch (ShopifyRateLimitException)
                {
                    await Task.Delay(RETRY_DELAY);
                }
            }
        }
    }
}
