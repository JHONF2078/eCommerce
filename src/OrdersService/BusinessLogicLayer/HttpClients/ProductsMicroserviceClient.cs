using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OrdersService.BusinessLogicLayer.DTO;
using Polly.Bulkhead;
using System.Net.Http.Json;
using System.Text.Json;

namespace OrdersMicroservice.BusinessLogicLayer.HttpClients;

public class ProductsMicroserviceClient
{
  private readonly HttpClient _httpClient;
    private readonly ILogger<ProductsMicroserviceClient> _logger;
    //la interfaz para la cache
    private readonly IDistributedCache _distributedCache;

public ProductsMicroserviceClient(HttpClient httpClient, 
    ILogger<ProductsMicroserviceClient> logger, 
    IDistributedCache distributedCache)
    {
    _httpClient = httpClient;
    _logger = logger;
    _distributedCache = distributedCache;
}

    public async Task<ProductDTO?> GetProductByProductID(Guid productID)
    {
        try
        {
            //Key: product:123
            //Value: { "ProductName: "...", ...}
            string cacheKey = $"product:{productID}";
            string? cachedProduct = await _distributedCache.GetStringAsync(cacheKey);

            if (cachedProduct != null)
            {
                ProductDTO? productFromCache = JsonSerializer.Deserialize<ProductDTO>(cachedProduct);
                return productFromCache;
            }

            HttpResponseMessage response = await _httpClient.GetAsync($"/gateway/products/search/product-id/{productID}");

            if (!response.IsSuccessStatusCode)
            {
                //si el servicio no está disponible, esta excepcion ya viene desde la politica de Fallback
                if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {

                    ProductDTO? productFromFallback = await response.Content.ReadFromJsonAsync<ProductDTO>();

                    if (productFromFallback == null)
                    {
                        throw new NotImplementedException("Fallback policy was not implement");
                    }

                    return productFromFallback;

                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new HttpRequestException("Bad request", null, System.Net.HttpStatusCode.BadRequest);
                }
                else
                {
                    throw new HttpRequestException($"Http request failed with status code {response.StatusCode}");
                }
            }

            ProductDTO? product = await response.Content.ReadFromJsonAsync<ProductDTO>();

            if (product == null)
            {
                throw new ArgumentException("Invalid Product ID");
            }

            //Key: product:{productID}
            //Value: { "ProductName": "..", ..}
            string productJson = JsonSerializer.Serialize(product);

            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
              //el producto se borra a los  5 minutos
              .SetAbsoluteExpiration(TimeSpan.FromSeconds(300));
              //si nadie lo consulta en 100 segundos, se borra antes de los 5 minutos
              //si alguien lo consulta antes de los 100 segundos, el contador se reinicia
              //y vuelve a contar los 100 segundos
              //.SetSlidingExpiration(TimeSpan.FromSeconds(100));

            string cacheKeyToWrite = $"product:{productID}";

            await _distributedCache.SetStringAsync(cacheKeyToWrite, productJson, options);


            return product;
        }
        catch (BulkheadRejectedException ex)
        {
            _logger.LogError(ex, "Bulkhead isolation blocks the request since the request queue is full");

            return new ProductDTO(
              ProductID: Guid.NewGuid(),
              ProductName: "Temporarily Unavailable (Bulkhead)",
              Category: "Temporarily Unavailable (Bulkhead)",
              UnitPrice: 0,
              QuantityInStock: 0);
        }
    }
}