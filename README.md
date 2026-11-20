# K Collective: first prototype

One working flow: find a photocard through an album edition's **possible pulls** or a **member's card grid**, add copies, and see the quantity persist. Everything runs as a local demo collection. It has no accounts, sync, catalog publishing, scanning, pricing or trading yet.

## Layout

| Project | What it is |
|---|---|
| `src/KCollective.Core` | Catalog model, seed data, `CollectionService` (one row per exact variant + quantity, member completion). No UI dependencies. |
| `src/KCollective.UI` | Razor class library holding **all screens, components, CSS and sample images**. Shared by every host. |
| `src/KCollective.Web` | Blazor WebAssembly host. The collection persists in `localStorage`. |
| `src/KCollective.Maui` | .NET MAUI Blazor Hybrid host (Android, plus iOS when built on a Mac). The collection persists as a JSON file in app data. |
| `tests/KCollective.Core.Tests` | xUnit tests for the collection rules. |

## Run in the browser

Requires the .NET 10 SDK.

```bash
dotnet run --project src/KCollective.Web --urls http://localhost:5156
```

Open http://localhost:5156. The demo flow:

1. **Album** → *Afterglow · Photobook Ver. A* → swipe the *Possible pulls* row → tap **Card 1**.
2. Press **+** three times, then refresh: the card shows a small **×3** in its bottom-right corner.
3. **All Yuna cards** → the header reads **1 / 5 variants** and the same card shows ×3 in the grid.

To reset the demo, clear site data, or run `localStorage.removeItem('kcollective.collection.v1')` in the console.

```bash
dotnet test
```

## Run on an Android phone (MAUI Blazor Hybrid)

One-time setup:

```bash
dotnet workload install maui-android
```

You also need a JDK 17 and the Android SDK (Android Studio installs both; or run `dotnet build src/KCollective.Maui -t:InstallAndroidDependencies -f net10.0-android -p:AndroidSdkDirectory=<path> -p:JavaSdkDirectory=<path> -p:AcceptAndroidSDKLicenses=True`). Turn on USB debugging on the phone, plug it in, and confirm `adb devices` lists it. Then:

```bash
dotnet build src/KCollective.Maui -t:Run -f net10.0-android
```

The MAUI project is deliberately left out of `KCollective.slnx` so that `dotnet build` and `dotnet test` work on machines without mobile workloads.

## Data model notes

- `PhotocardVariant` is the exact variant. A pre-order benefit with the same photo is a separate variant.
- *Possible pulls* = the edition's variants with `Source = AlbumPull`. The member grid shows every cataloged variant for the member, including the pre-order benefit, which is why it reads `/ 5` while the album shows 4 pulls.
- The collection holds one `CollectionEntry { VariantId, Quantity }` per variant. Completion counts variants with quantity > 0, so three copies of one card advance completion by one.
- All seed data is fictional and labeled *Sample* on the cards, the cover and the group name.
- `PullRow` is self-contained, so the cylindrical carousel can replace it without touching the album page.
