namespace KCollective.Core.Catalog;

/// <summary>
/// Demo catalog. Everything here is fictional sample data, labeled as such in the UI and on the images.
/// </summary>
public static class SeedCatalog
{
    public const string GroupId = "sample-group";
    public const string MemberId = "sample-member-yuna";
    public const string AlbumId = "sample-album-afterglow";
    public const string EditionId = "sample-afterglow-photobook-a";

    public static InMemoryCatalog Create()
    {
        var group = new Group(GroupId, "LUMEN (Sample Group)");
        var member = new Member(MemberId, GroupId, "Yuna", "#7c5cff");
        var album = new Album(AlbumId, GroupId, "Afterglow", new DateOnly(2026, 9, 1));

        var edition = new AlbumEdition(
            EditionId,
            AlbumId,
            "Photobook Ver. A",
            CoverImage: "_content/KCollective.UI/img/sample/cover-afterglow-a.svg",
            SpineColor: "#3a2a7a",
            Inclusions:
            [
                new("Photobook", "book", "80 pages"),
                new("CD", "cd", "1 disc"),
                new("Photocard", "card", "1 random of 4"),
                new("Postcard", "postcard", "1 random"),
                new("Sticker", "sticker", "1 sheet"),
            ]);

        PhotocardVariant Card(string slug, string name, CardSource source, string? detail = null) => new(
            Id: $"{EditionId}-{MemberId}-{slug}",
            MemberId: MemberId,
            EditionId: EditionId,
            Name: name,
            Source: source,
            SourceDetail: detail,
            Image: $"_content/KCollective.UI/img/sample/pc-yuna-{slug}.svg",
            WidthMm: 55,
            HeightMm: 85,
            IsSample: true);

        var variants = new[]
        {
            Card("pc1", "Photobook A · Card 1", CardSource.AlbumPull),
            Card("pc2", "Photobook A · Card 2", CardSource.AlbumPull),
            Card("selfie", "Photobook A · Selfie", CardSource.AlbumPull),
            Card("polaroid", "Photobook A · Polaroid", CardSource.AlbumPull),
            Card("pob", "Pre-order Benefit", CardSource.PreOrderBenefit, "Sample Records store"),
        };

        return new InMemoryCatalog([group], [member], [album], [edition], variants);
    }
}
