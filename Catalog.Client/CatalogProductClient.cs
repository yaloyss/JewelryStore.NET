using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JewelryStore.Catalog.Client;

public class CatalogProductClient : ICatalogProductClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CatalogProductClient> _logger;

    public CatalogProductClient(
        HttpClient httpClient,
        IOptions<CatalogClientOptions> options,
        ILogger<CatalogProductClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        if (_httpClient.BaseAddress == null && !string.IsNullOrWhiteSpace(options.Value.BaseUrl))
        {
            _httpClient.BaseAddress = new Uri(options.Value.BaseUrl.TrimEnd('/') + "/");
        }
    }

    public async Task<CatalogProductDto?> GetByIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        if (productId <= 0)
        {
            return null;
        }

        try
        {
            using var response = await _httpClient.GetAsync($"api/Products/{productId}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CatalogProductDto>(cancellationToken: cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to fetch product {ProductId} from Catalog service", productId);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int productId, CancellationToken cancellationToken = default)
    {
        var product = await GetByIdAsync(productId, cancellationToken);
        return product != null;
    }
}
