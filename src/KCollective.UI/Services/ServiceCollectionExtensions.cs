using KCollective.Core.Catalog;
using KCollective.Core.Collection;
using Microsoft.Extensions.DependencyInjection;

namespace KCollective.UI.Services;

public static class ServiceCollectionExtensions
{
    /// <summary>Registers the seed catalog and collection service. The host supplies the <see cref="ICollectionStore"/>.</summary>
    public static IServiceCollection AddKCollective<TStore>(this IServiceCollection services)
        where TStore : class, ICollectionStore
    {
        services.AddSingleton<ICatalog>(_ => SeedCatalog.Create());
        services.AddSingleton<ICollectionStore, TStore>();
        services.AddSingleton<CollectionService>();
        return services;
    }
}
