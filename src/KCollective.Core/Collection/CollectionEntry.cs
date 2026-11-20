namespace KCollective.Core.Collection;

/// <summary>One row per exact photocard variant the user owns, with how many copies.</summary>
public sealed record CollectionEntry(string VariantId, int Quantity, DateTimeOffset UpdatedAt);

public sealed record Completion(int Owned, int Total)
{
    public double Fraction => Total == 0 ? 0 : (double)Owned / Total;
}
