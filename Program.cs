using GPTCvAssistant;
using GPTCvAssistant.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    .AddJsonOptions(options =>
    {
        // Configure enum conversion for proper serialization/deserialization
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Add HttpClient service
builder.Services.AddHttpClient();

// Configure OpenAI settings
builder.Services.Configure<OpenAISettings>(
    builder.Configuration.GetSection("OpenAI"));

// Configure Gemini settings using the nested class
builder.Services.Configure<GeminiService.GeminiSettings>(
    builder.Configuration.GetSection("Gemini"));

// Register services
builder.Services.AddSingleton<OpenAiService>();
builder.Services.AddSingleton<GeminiService>();
builder.Services.AddSingleton<JobMatchingAgent>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

app.UseSession(); // Add before app.UseRouting()


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Chat}/{action=Index}/{id?}");

app.Run();
