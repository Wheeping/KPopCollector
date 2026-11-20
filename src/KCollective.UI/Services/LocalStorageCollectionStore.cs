using System.Text.Json;
using KCollective.Core.Collection;
using Microsoft.JSInterop;

namespace KCollective.UI.Services;

/// <summary>Stores the demo collection in the browser's localStorage.</summary>
public sealed class LocalStorageCollectionStore(IJSRuntime js) : ICollectionStore
{
    public const string Key = "kcollective.collection.v1";

    public async Task<IReadOnlyList<CollectionEntry>> LoadAsync()
    {
        var json = await js.InvokeAsync<string?>("localStorage.getItem", Key);
        return CollectionJson.Deserialize(json);
    }

    public async Task SaveAsync(IReadOnlyList<CollectionEntry> entries) =>
        await js.InvokeVoidAsync("localStorage.setItem", Key, CollectionJson.Serialize(entries));
}

/// <summary>Versioned JSON shape shared by every local store.</summary>
public static class CollectionJson
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    private sealed record Document(int Version, List<CollectionEntry> Entries);

    public static string Serialize(IReadOnlyList<CollectionEntry> entries) =>
        JsonSerializer.Serialize(new Document(1, entries.ToList()), Options);

    public static IReadOnlyList<CollectionEntry> Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return [];
        try { return JsonSerializer.Deserialize<Document>(json, Options)?.Entries ?? []; }
        catch (JsonException) { return []; } // Corrupt demo data: start fresh rather than crash.
    }
}
