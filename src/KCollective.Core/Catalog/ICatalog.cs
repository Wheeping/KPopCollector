namespace KCollective.Core.Catalog;

/// <summary>Read-only catalog. The seed implementation is in-memory; a published catalog can replace it later.</summary>
public interface ICatalog
{
    Group? GetGroup(string id);
    Member? GetMember(string id);
    Album? GetAlbum(string id);
    AlbumEdition? GetEdition(string id);
    PhotocardVariant? GetVariant(string id);

    IReadOnlyList<AlbumEdition> Editions { get; }
    IReadOnlyList<Member> Members { get; }

    /// <summary>Cards that can be pulled from a copy of this edition.</summary>
    IReadOnlyList<PhotocardVariant> PossiblePulls(string editionId);

    /// <summary>Every cataloged variant for the member, across all sources.</summary>
    IReadOnlyList<PhotocardVariant> VariantsForMember(string memberId);
}

public sealed class InMemoryCatalog : ICatalog
{
    private readonly Dictionary<string, Group> _groups;
    private readonly Dictionary<string, Member> _members;
    private readonly Dictionary<string, Album> _albums;
    private readonly Dictionary<string, AlbumEdition> _editions;
    private readonly List<PhotocardVariant> _variants;
    private readonly Dictionary<string, PhotocardVariant> _variantsById;

    public InMemoryCatalog(
        IEnumerable<Group> groups,
        IEnumerable<Member> members,
        IEnumerable<Album> albums,
        IEnumerable<AlbumEdition> editions,
        IEnumerable<PhotocardVariant> variants)
    {
        _groups = groups.ToDictionary(g => g.Id);
        _members = members.ToDictionary(m => m.Id);
        _albums = albums.ToDictionary(a => a.Id);
        _editions = editions.ToDictionary(e => e.Id);
        _variants = variants.ToList();
        _variantsById = _variants.ToDictionary(v => v.Id);
    }

    public Group? GetGroup(string id) => _groups.GetValueOrDefault(id);
    public Member? GetMember(string id) => _members.GetValueOrDefault(id);
    public Album? GetAlbum(string id) => _albums.GetValueOrDefault(id);
    public AlbumEdition? GetEdition(string id) => _editions.GetValueOrDefault(id);
    public PhotocardVariant? GetVariant(string id) => _variantsById.GetValueOrDefault(id);

    public IReadOnlyList<AlbumEdition> Editions => _editions.Values.ToList();
    public IReadOnlyList<Member> Members => _members.Values.ToList();

    public IReadOnlyList<PhotocardVariant> PossiblePulls(string editionId) =>
        _variants.Where(v => v.EditionId == editionId && v.Source == CardSource.AlbumPull).ToList();

    public IReadOnlyList<PhotocardVariant> VariantsForMember(string memberId) =>
        _variants.Where(v => v.MemberId == memberId).ToList();
}
