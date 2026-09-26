using Kontursvet.Api.Endpoints;
using Kontursvet.Application;
using Kontursvet.Infrastructure;
using Kontursvet.Configuration;
using Microsoft.Extensions.FileProviders;
using Dapper;

var builder = WebApplication.CreateBuilder(args);

DefaultTypeMap.MatchNamesWithUnderscores = true;

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

app.UseAuthentication();
app.UseAuthorization();

app.UseCors();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapAdminEndpoints();
app.MapAuthEndpoints();
app.MapPortfolioCardsEndpoints();
app.MapPortfolioCardViewsEndpoints();
app.MapLeadEndpoints();
app.MapFileEndpoints();

app.Run();