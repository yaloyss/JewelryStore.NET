using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JewelryStore.Catalog.Client;

public static class CatalogClientServiceCollectionExtensions
{
    public static IServiceCollection AddCatalogProductClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<CatalogClientOptions>(configuration.GetSection(CatalogClientOptions.SectionName));

        services.AddHttpClient<ICatalogProductClient, CatalogProductClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<CatalogClientOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}
