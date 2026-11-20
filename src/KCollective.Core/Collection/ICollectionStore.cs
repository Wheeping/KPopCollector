namespace KCollective.Core.Collection;

/// <summary>Persists the local collection. Implemented per host (browser storage, device file, later a sync service).</summary>
public interface ICollectionStore
{
    Task<IReadOnlyList<CollectionEntry>> LoadAsync();
    Task SaveAsync(IReadOnlyList<CollectionEntry> entries);
}

public sealed class InMemoryCollectionStore : ICollectionStore
{
    private IReadOnlyList<CollectionEntry> _entries = [];
    public Task<IReadOnlyList<CollectionEntry>> LoadAsync() => Task.FromResult(_entries);
    public Task SaveAsync(IReadOnlyList<CollectionEntry> entries)
    {
        _entries = entries.ToList();
        return Task.CompletedTask;
    }
}
