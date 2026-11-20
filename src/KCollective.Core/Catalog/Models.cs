namespace KCollective.Core.Catalog;

public sealed record Group(string Id, string Name);

public sealed record Member(string Id, string GroupId, string StageName, string AccentColor);

/// <summary>A release (e.g. a mini album). Editions are the physical versions you can buy.</summary>
public sealed record Album(string Id, string GroupId, string Title, DateOnly ReleaseDate);

public sealed record Inclusion(string Label, string Kind, string Detail);

/// <summary>One physical version of an album (e.g. "Photobook Ver. A").</summary>
public sealed record AlbumEdition(
    string Id,
    string AlbumId,
    string Name,
    string CoverImage,
    string SpineColor,
    IReadOnlyList<Inclusion> Inclusions);

public enum CardSource
{
    /// <summary>Randomly inserted in the album itself – appears in the edition's possible pulls.</summary>
    AlbumPull,
    /// <summary>Given by a retailer alongside the album – cataloged for the member, but not a pull.</summary>
    PreOrderBenefit,
}

/// <summary>
/// An exact photocard variant. Two cards with the same photo but a different
/// back, finish or source are separate variants with separate ids.
/// </summary>
public sealed record PhotocardVariant(
    string Id,
    string MemberId,
    string EditionId,
    string Name,
    CardSource Source,
    string? SourceDetail,
    string Image,
    int WidthMm,
    int HeightMm,
    bool IsSample);
