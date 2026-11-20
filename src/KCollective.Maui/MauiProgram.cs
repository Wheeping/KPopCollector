using KCollective.Maui.Services;
using KCollective.UI.Services;
using Microsoft.Extensions.Logging;

namespace KCollective.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

		// Phone host: the demo collection persists as a JSON file in the app's private data folder.
		builder.Services.AddKCollective<FileCollectionStore>();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
