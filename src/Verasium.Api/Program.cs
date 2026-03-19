using Verasium.Core;

var builder = WebApplication.CreateBuilder(args);

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
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();
app.MapControllers();

app.Run();
