using GPTCvAssistant;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

// Add HttpClient service
builder.Services.AddHttpClient();

builder.Services.Configure<OpenAISettings>(
    builder.Configuration.GetSection("OpenAI"));

builder.Services.AddSingleton<OpenAiService>();
builder.Services.Configure<GeminiService.GeminiSettings>(
    builder.Configuration.GetSection("Gemini"));

builder.Services.AddSingleton<GeminiService>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

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
