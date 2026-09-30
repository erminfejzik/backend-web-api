var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", () => System.DateTime.UtcNow.ToString(
    "yyyy-MM-dd HH:mm:ss",
    System.Globalization.CultureInfo.CurrentCulture));

await app.RunAsync();
