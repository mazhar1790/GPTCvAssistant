using GPTCvAssistant.Configuration;
using GPTCvAssistant.Middleware;
using GPTCvAssistant.Services;
using GPTCvAssistant.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Serilog.Events;

// Configure Serilog early
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog for logging
    builder.Host.UseSerilog();

    // Add services to the container
    builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    .AddJsonOptions(options =>
    {
        // Configure JSON serialization
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

    // Configure HTTP clients
    builder.Services.AddHttpClient<GeminiService>(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(120);
        client.DefaultRequestHeaders.Add("User-Agent", "GPTCvAssistant/2.0");
    });

    builder.Services.AddHttpClient<OpenAiService>(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(120);
        client.DefaultRequestHeaders.Add("User-Agent", "GPTCvAssistant/2.0");
    });

    // Configure settings using the options pattern
    builder.Services.Configure<OpenAISettings>(
        builder.Configuration.GetSection(OpenAISettings.SectionName));
    builder.Services.Configure<GeminiSettings>(
        builder.Configuration.GetSection(GeminiSettings.SectionName));

    // Add response compression
    builder.Services.AddResponseCompression(opts =>
    {
        opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            new[] { "application/octet-stream" });
    });

    // Register services with dependency injection
    builder.Services.AddScoped<IAiService, GeminiService>();
    builder.Services.AddScoped<OpenAiService>();
    builder.Services.AddScoped<IJobMatchingService, JobMatchingService>();

    // Add memory cache for performance
    builder.Services.AddMemoryCache();

    // Enhanced session configuration
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.Cookie.Name = "GPTCvAssistant.Session";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.IdleTimeout = TimeSpan.FromMinutes(60);
    });

    // Add health checks with dependencies
    builder.Services.AddHealthChecks()
        .AddCheck<GeminiHealthCheck>("gemini")
        .AddCheck<OpenAiHealthCheck>("openai");

    // Add anti-forgery
    builder.Services.AddAntiforgery(options =>
    {
        options.HeaderName = "X-CSRF-TOKEN";
        options.Cookie.Name = "__RequestVerificationToken";
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

    // Configure forwarded headers for reverse proxy scenarios
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline
    app.UseForwardedHeaders();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
        
        // Add security headers
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("X-Frame-Options", "DENY");
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
            await next();
        });
    }
    else
    {
        app.UseDeveloperExceptionPage();
    }

    // Add response compression
    app.UseResponseCompression();

    // Add global exception handling middleware
    app.UseMiddleware<GlobalExceptionMiddleware>();

    app.UseHttpsRedirection();
    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            // Cache static files for 30 days
            const int durationInSeconds = 60 * 60 * 24 * 30;
            ctx.Context.Response.Headers.Add("Cache-Control", $"public,max-age={durationInSeconds}");
        }
    });

    app.UseRouting();

    // Add session middleware before authorization
    app.UseSession();
    app.UseAuthorization();

    // Map health check endpoint with detailed output
    app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(x => new
                {
                    name = x.Key,
                    status = x.Value.Status.ToString(),
                    description = x.Value.Description,
                    duration = x.Value.Duration.ToString()
                }),
                totalDuration = report.TotalDuration.ToString()
            });
            await context.Response.WriteAsync(result);
        }
    });

    // Configure routing with lowercase URLs
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Chat}/{action=Index}/{id?}");

    Log.Information("Starting GPT CV Assistant application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
