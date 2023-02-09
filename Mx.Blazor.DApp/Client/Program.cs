using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Mx.Blazor.DApp.Client;
using Mx.Blazor.DApp.Client.Services;
using Mx.Blazor.DApp.Client.Services.Containers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<ICopyService, CopyService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddScoped<WalletProviderContainer>();
builder.Services.AddScoped<TransactionsContainer>();
builder.Services.AddScoped<AccountContainer>();

await builder.Build().RunAsync();
