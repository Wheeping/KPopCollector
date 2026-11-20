using KCollective.Core.Catalog;

namespace KCollective.Core.Collection;

public sealed class CollectionService(ICatalog catalog, ICollectionStore store, TimeProvider? clock = null)
{
    private readonly TimeProvider _clock = clock ?? TimeProvider.System;
    private readonly Dictionary<string, CollectionEntry> _entries = new();
    private readonly SemaphoreSlim _gate = new(1, 1);
    private bool _loaded;

    public event Action? Changed;

    public IReadOnlyCollection<CollectionEntry> Entries => _entries.Values;

    public async Task EnsureLoadedAsync()
    {
        if (_loaded) return;
        await _gate.WaitAsync();
        try
        {
            if (_loaded) return;
            foreach (var e in await store.LoadAsync())
            {
                // Ignore rows for variants the catalog no longer knows, and merge any duplicates.
                if (catalog.GetVariant(e.VariantId) is null || e.Quantity <= 0) continue;
                _entries[e.VariantId] = _entries.TryGetValue(e.VariantId, out var existing)
                    ? existing with { Quantity = existing.Quantity + e.Quantity }
                    : e;
            }
            _loaded = true;
        }
        finally { _gate.Release(); }
    }

    public int QuantityOf(string variantId) =>
        _entries.TryGetValue(variantId, out var e) ? e.Quantity : 0;

    public Task AddCopiesAsync(string variantId, int copies = 1)
    {
        if (copies < 1) throw new ArgumentOutOfRangeException(nameof(copies));
        return SetQuantityAsync(variantId, QuantityOf(variantId) + copies);
    }

    public Task RemoveCopyAsync(string variantId) =>
        SetQuantityAsync(variantId, Math.Max(0, QuantityOf(variantId) - 1));

    public async Task SetQuantityAsync(string variantId, int quantity)
    {
        if (quantity < 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (catalog.GetVariant(variantId) is null)
            throw new ArgumentException($"Unknown photocard variant '{variantId}'.", nameof(variantId));

        await EnsureLoadedAsync();
        await _gate.WaitAsync();
        try
        {
            if (quantity == 0) _entries.Remove(variantId);
            else _entries[variantId] = new CollectionEntry(variantId, quantity, _clock.GetUtcNow());
            await store.SaveAsync(_entries.Values.OrderBy(e => e.VariantId, StringComparer.Ordinal).ToList());
        }
        finally { _gate.Release(); }
        Changed?.Invoke();
    }

    /// <summary>Distinct variants owned (any quantity) out of all cataloged variants for the member.</summary>
    public Completion MemberCompletion(string memberId)
    {
        var variants = catalog.VariantsForMember(memberId);
        return new Completion(variants.Count(v => QuantityOf(v.Id) > 0), variants.Count);
    }
}
