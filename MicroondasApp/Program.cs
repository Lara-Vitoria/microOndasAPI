using MicroondasApp.Models;
using MicroondasApp.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MicroOndasDb>(opt => opt.UseInMemoryDatabase("MicroOndas"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

var caminho = app.MapGroup("/microondas");

caminho.MapGet("/", async (MicroOndasDb db) =>
    await db.MicroOndas.ToListAsync());

caminho.MapPost("/", async (MicroOndas microOndas, MicroOndasDb microOndasdb) =>
{
    microOndasdb.MicroOndas.Add(microOndas);
    await microOndasdb.SaveChangesAsync();

    return Results.Created($"/microondas/", microOndas);
});

app.Run();
