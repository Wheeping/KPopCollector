using KCollective.Core.Collection;
using KCollective.UI.Services;

namespace KCollective.Maui.Services;

/// <summary>Stores the demo collection in the app's private data directory (survives app restarts).</summary>
public sealed class FileCollectionStore : ICollectionStore
{
	private readonly string _path = Path.Combine(FileSystem.AppDataDirectory, "collection.v1.json");

	public async Task<IReadOnlyList<CollectionEntry>> LoadAsync() =>
		File.Exists(_path) ? CollectionJson.Deserialize(await File.ReadAllTextAsync(_path)) : [];

	public async Task SaveAsync(IReadOnlyList<CollectionEntry> entries)
	{
		// Write then swap, so a crash mid-write can't leave a half-written file.
		var tmp = _path + ".tmp";
		await File.WriteAllTextAsync(tmp, CollectionJson.Serialize(entries));
		File.Move(tmp, _path, overwrite: true);
	}
}
