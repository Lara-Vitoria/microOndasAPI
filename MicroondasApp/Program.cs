using MicroondasApp.Application;
using MicroondasApp.Application.Contratos;
using MicroondasApp.Persistence;
using MicroondasApp.Service;
using Microsoft.EntityFrameworkCore;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MicroOndasDb>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("MicroOndasDb")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("https://localhost:7252");
                      });
});

builder.Services.AddControllers();

builder.Services.AddScoped<IMicroOndasService, MicroOndasService>();
builder.Services.AddScoped<IProgramaService, ProgramaService>();
builder.Services.AddScoped<IStatusService, StatusService>();

var app = builder.Build();

app.UseCors(builder => builder
.AllowAnyOrigin()
.AllowAnyMethod()
.AllowAnyHeader()
);

app.MapControllers();
app.Run();