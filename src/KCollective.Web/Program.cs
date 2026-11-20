using KCollective.UI.Services;
using KCollective.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Browser host: the demo collection persists in localStorage.
builder.Services.AddKCollective<LocalStorageCollectionStore>();

await builder.Build().RunAsync();
