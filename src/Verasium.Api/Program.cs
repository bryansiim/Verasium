using Verasium.Core;

var builder = WebApplication.CreateBuilder(args);

// Porta dinamica (Railway define via variavel PORT)
var port = Environment.GetEnvironmentVariable("PORT");
if (port != null)
{
    builder.WebHost.UseUrls($"http://+:{port}");
}

// Limite global de upload: 30MB (para suportar videos)
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 30_000_000;
});

builder.Services.AddControllers();
builder.Services.AddSingleton<GeminiAnalyzer>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var origins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS");
        if (!string.IsNullOrEmpty(origins))
        {
            policy.WithOrigins(origins.Split(','))
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

var app = builder.Build();

app.UseCors();
app.MapControllers();

app.Run();
