using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Mx.Blazor.DApp.Client.Services;
using Mx.Blazor.DApp.Client.Services.Containers;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Mx.Blazor.DApp.Client.Services.Wallet;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<ICopyService, CopyService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddScoped<WalletManagerService>();
builder.Services.AddScoped<WalletProviderContainer>();
builder.Services.AddScoped<NativeAuthService>();
builder.Services.AddScoped<TransactionsContainer>();
builder.Services.AddScoped<AccountContainer>();

await builder.Build().RunAsync();
