using GPTCvAssistant.Configuration;
using GPTCvAssistant.Middleware;
using GPTCvAssistant.Services;
using GPTCvAssistant.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    .AddJsonOptions(options =>
    {
        // Configure enum conversion for proper serialization/deserialization
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Configure HTTP client
builder.Services.AddHttpClient<GeminiService>();
builder.Services.AddHttpClient<OpenAiService>();

// Configure settings using the constants for section names
builder.Services.Configure<OpenAISettings>(
    builder.Configuration.GetSection(OpenAISettings.SectionName));

builder.Services.Configure<GeminiSettings>(
    builder.Configuration.GetSection(GeminiSettings.SectionName));

// Register services with dependency injection
// Use Gemini as the primary AI service, with fallback to OpenAI if needed
builder.Services.AddScoped<IAiService, GeminiService>();
builder.Services.AddScoped<OpenAiService>(); // Keep available for specific use cases
builder.Services.AddScoped<IJobMatchingService, JobMatchingService>();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set reasonable timeout
});

// Add logging
builder.Services.AddLogging();

// Add health checks for monitoring
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// Add global exception handling middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Add session middleware before authorization
app.UseSession();
app.UseAuthorization();

// Map health check endpoint
app.MapHealthChecks("/health");

// Configure routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Chat}/{action=Index}/{id?}");

app.Run();
