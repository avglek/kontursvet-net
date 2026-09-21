using Kontursvet.Api.Endpoints;
using Kontursvet.Application;
using Kontursvet.Infrastructure;
using Kontursvet.Configuration;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddApiServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kontursvet API v1");
        c.RoutePrefix = "swagger";
    });
}
;


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
    RequestPath = "",
    OnPrepareResponse = ctx =>
    {
        // Кэш на год — файлы с Guid в имени, никогда не перезаписываются
        ctx.Context.Response.Headers.Append(
            "Cache-Control", "public, max-age=31536000, immutable");
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.UseCors();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPortfolioCardsEndpoints();
app.MapPortfolioCardViewsEndpoints();
app.MapLeadEndpoints();
app.MapFileEndpoints();

app.Run();