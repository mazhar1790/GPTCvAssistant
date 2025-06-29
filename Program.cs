
using GPTCvAssistant;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.Configure<GPTCvAssistant.OpenAISettings>(
    builder.Configuration.GetSection("OpenAI"));

builder.Services.AddSingleton<GPTCvAssistant.OpenAiService>();
builder.Services.Configure<GeminiSettings>(
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
