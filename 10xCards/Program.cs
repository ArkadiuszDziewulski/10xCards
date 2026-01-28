using _10xCards;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Supabase;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var url = builder.Configuration["Supabase:Url"] ?? "";
var key = builder.Configuration["Supabase:Key"] ?? "";
builder.Services.AddScoped(_ => new Supabase.Client(url, key, new SupabaseOptions { AutoConnectRealtime = true }));

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();

