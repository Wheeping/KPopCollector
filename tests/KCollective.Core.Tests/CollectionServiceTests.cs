using KCollective.Core.Catalog;
using KCollective.Core.Collection;

namespace KCollective.Core.Tests;

public class CollectionServiceTests
{
    private readonly InMemoryCatalog _catalog = SeedCatalog.Create();
    private readonly InMemoryCollectionStore _store = new();

    private CollectionService NewService() => new(_catalog, _store);

    private PhotocardVariant FirstPull => _catalog.PossiblePulls(SeedCatalog.EditionId)[0];

    [Fact]
    public void Same_variant_is_reachable_from_album_pulls_and_member_grid()
    {
        var fromAlbum = _catalog.PossiblePulls(SeedCatalog.EditionId).Select(v => v.Id);
        var fromMember = _catalog.VariantsForMember(SeedCatalog.MemberId).Select(v => v.Id);
        Assert.Contains(FirstPull.Id, fromMember);
        Assert.Subset(fromMember.ToHashSet(), fromAlbum.ToHashSet());
    }

    [Fact]
    public void Pre_order_benefit_counts_for_member_but_is_not_a_possible_pull()
    {
        var pob = _catalog.VariantsForMember(SeedCatalog.MemberId).Single(v => v.Source == CardSource.PreOrderBenefit);
        Assert.DoesNotContain(pob, _catalog.PossiblePulls(SeedCatalog.EditionId));
    }

    [Fact]
    public void All_seed_cards_are_labeled_samples()
    {
        Assert.All(_catalog.VariantsForMember(SeedCatalog.MemberId), v => Assert.True(v.IsSample));
    }

    [Fact]
    public async Task Adding_three_copies_stores_one_row_with_quantity_three()
    {
        var svc = NewService();
        await svc.AddCopiesAsync(FirstPull.Id);
        await svc.AddCopiesAsync(FirstPull.Id);
        await svc.AddCopiesAsync(FirstPull.Id);

        var saved = await _store.LoadAsync();
        var row = Assert.Single(saved);
        Assert.Equal(FirstPull.Id, row.VariantId);
        Assert.Equal(3, row.Quantity);
    }

    [Fact]
    public async Task Three_copies_of_one_variant_advance_completion_by_one()
    {
        var svc = NewService();
        await svc.EnsureLoadedAsync();
        var before = svc.MemberCompletion(SeedCatalog.MemberId);

        await svc.AddCopiesAsync(FirstPull.Id, 3);

        var after = svc.MemberCompletion(SeedCatalog.MemberId);
        Assert.Equal(before.Owned + 1, after.Owned);
        Assert.Equal(before.Total, after.Total);
        Assert.Equal(5, after.Total);
    }

    [Fact]
    public async Task Quantity_survives_a_reload()
    {
        await NewService().AddCopiesAsync(FirstPull.Id, 3);

        var reloaded = NewService(); // same store, fresh service = "refresh"
        await reloaded.EnsureLoadedAsync();
        Assert.Equal(3, reloaded.QuantityOf(FirstPull.Id));
        Assert.Equal(1, reloaded.MemberCompletion(SeedCatalog.MemberId).Owned);
    }

    [Fact]
    public async Task Removing_last_copy_deletes_the_row_and_completion_drops()
    {
        var svc = NewService();
        await svc.AddCopiesAsync(FirstPull.Id);
        await svc.RemoveCopyAsync(FirstPull.Id);

        Assert.Empty(await _store.LoadAsync());
        Assert.Equal(0, svc.MemberCompletion(SeedCatalog.MemberId).Owned);
    }

    [Fact]
    public async Task Load_merges_duplicate_rows_and_drops_unknown_variants()
    {
        var now = DateTimeOffset.UtcNow;
        await _store.SaveAsync([
            new(FirstPull.Id, 1, now),
            new(FirstPull.Id, 2, now),
            new("no-such-card", 4, now),
        ]);
        var svc = NewService();
        await svc.EnsureLoadedAsync();
        Assert.Equal(3, svc.QuantityOf(FirstPull.Id));
        Assert.Single(svc.Entries);
    }

    [Fact]
    public async Task Unknown_variant_is_rejected()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => NewService().AddCopiesAsync("nope"));
    }
}
