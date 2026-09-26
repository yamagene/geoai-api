using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// API の手動確認用。開発環境でのみ公開する（spec.md 2 章、4.3）
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();

app.Run();

// テストから WebApplicationFactory<Program> で参照するため
public partial class Program;
