using Bff.Services;
using ThatInterpretingAgency.Core.Application.Common;
using ThatInterpretingAgency.Core.Application.Services;
using ThatInterpretingAgency.Infrastructure.Persistence;
using ThatInterpretingAgency.Infrastructure.Persistence.Repositories;
using ThatInterpretingAgency.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add HTTP Client for reverse proxy
builder.Services.AddHttpClient("ApiProxy", client =>
{
    client.BaseAddress = new Uri("https://localhost:7058/");
});

// Add DbContext
builder.Services.AddDbContext<ThatInterpretingAgencyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repositories
builder.Services.AddScoped<IAgencyRepository, AgencyRepository>();
builder.Services.AddScoped<IInterpreterRepository, InterpreterRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAgencyStaffRepository, AgencyStaffRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInterpreterRequestRepository, InterpreterRequestRepository>();
builder.Services.AddScoped<IAvailabilitySlotRepository, AvailabilitySlotRepository>();

// Add Calendar Integration services
builder.Services.AddScoped<ICalendarIntegrationService, CalendarIntegrationService>();

// Add BFF Services
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAgencyService, AgencyService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:7263", "http://localhost:5096", "https://localhost:7001", "http://localhost:5001")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorFrontend");
app.UseAuthorization();

// Custom reverse proxy middleware
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    
    // Check if this is an API request that should be proxied
    if (path != null && path.StartsWith("/api/") && 
        !path.StartsWith("/api/bff/") && 
        !path.StartsWith("/api/proxy-test/"))
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Proxying request: {Method} {Path} to API", 
            context.Request.Method, path);
        
        // Proxy the request to the main API
        var httpClientFactory = context.RequestServices.GetRequiredService<IHttpClientFactory>();
        var httpClient = httpClientFactory.CreateClient("ApiProxy");
        
        // Create the request
        var requestMessage = new HttpRequestMessage();
        var requestMethod = context.Request.Method;
        requestMessage.Method = new HttpMethod(requestMethod);
        requestMessage.RequestUri = new Uri(httpClient.BaseAddress + path.TrimStart('/'));
        
        // Copy headers
        foreach (var header in context.Request.Headers)
        {
            if (!header.Key.StartsWith("Host", StringComparison.OrdinalIgnoreCase))
            {
                requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }
        
        // Copy query string
        if (context.Request.QueryString.HasValue)
        {
            var uriBuilder = new UriBuilder(requestMessage.RequestUri);
            uriBuilder.Query = context.Request.QueryString.Value;
            requestMessage.RequestUri = uriBuilder.Uri;
        }
        
        // Copy body for POST/PUT requests
        if (requestMethod == "POST" || requestMethod == "PUT" || requestMethod == "PATCH")
        {
            var streamContent = new StreamContent(context.Request.Body);
            requestMessage.Content = streamContent;
        }
        
        try
        {
            var response = await httpClient.SendAsync(requestMessage);
            
            // Copy response back to the client
            context.Response.StatusCode = (int)response.StatusCode;
            foreach (var header in response.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }
            
            if (response.Content != null)
            {
                foreach (var header in response.Content.Headers)
                {
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                }
                
                await response.Content.CopyToAsync(context.Response.Body);
            }
            
            return; // Don't continue to next middleware
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error proxying request to API");
            context.Response.StatusCode = 502; // Bad Gateway
            await context.Response.WriteAsync("Error proxying request to API");
            return;
        }
    }
    
    await next();
});

// Map BFF-specific controllers
app.MapControllers();

app.Run();
