using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using BlazorFrontend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add HttpClient for BFF communication
builder.Services.AddHttpClient("BffClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7002/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Add HttpClient for legacy API communication (for services not yet converted)
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5032/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Add named HttpClients
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BffClient"));
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient"));

// Register the BFF service
builder.Services.AddScoped<IBffService, BffService>();

// Register the legacy API service (for services not yet converted)
builder.Services.AddScoped<IApiService, ApiService>();

// Register domain services - now using BFF-based implementations
builder.Services.AddScoped<IAgencyService, BffAgencyService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IInterpreterRequestService, InterpreterRequestService>();
builder.Services.AddScoped<IBffDashboardService, BffDashboardService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
