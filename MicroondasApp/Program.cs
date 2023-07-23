using MicroondasApp.Models;
using MicroondasApp.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text;

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

var app = builder.Build();

app.UseCors(builder => builder
.AllowAnyOrigin()
.AllowAnyMethod()
.AllowAnyHeader()
);

var caminhoMicroondas = app.MapGroup("/microondas");

caminhoMicroondas.MapGet("/", GetMicroOndas);
caminhoMicroondas.MapPost("/", CriaMicroOndas);
caminhoMicroondas.MapPost("/ParaPausar", PararPausarAquecimento);
caminhoMicroondas.MapGet("/aquecimento/{tempo?}/{potencia?}", Aquecimento);
caminhoMicroondas.MapGet("/statusAquecimento/{tempo?}/{potencia?}/{stgAquecimento?}", StatusAquecimento);

var caminhoPrograma = app.MapGroup("/programa");

caminhoPrograma.MapGet("/", GetProgramas);
caminhoPrograma.MapGet("/{id}", GetProgramaById);
caminhoPrograma.MapPost("/", CriaPrograma);

app.Run();

static async Task<IResult> GetMicroOndas(MicroOndasDb microOndasdb)
{
    return TypedResults.Ok(await microOndasdb.MicroOndas.ToArrayAsync());
}

static async Task<IResult> GetProgramas(MicroOndasDb microOndasdb)
{
    return TypedResults.Ok(await microOndasdb.Programas.ToArrayAsync());
}

static async Task<IResult> GetProgramaById(int id, MicroOndasDb microOndasdb)
{
    var programa = await microOndasdb.Programas.FindAsync(id);

    var stg = StatusAquecimento(programa.Tempo, programa.Potencia, programa.StgAquecimento.ToString()).Result;

    return TypedResults.Ok(stg);
}

static async Task<IResult> CriaMicroOndas(MicroOndas microOndas, MicroOndasDb microOndasdb)
{
    if (VerificaIntervaloDeTempo(microOndas.Tempo))
        return TypedResults.BadRequest("O tempo deve ser entre 1 segundo e 2 minutos");

    if (VerificaIntervaloDePotencia(microOndas.Potencia))
        return TypedResults.BadRequest("A potencia deve ser entre 1 e 10");

    var ultimoMicroondas = await microOndasdb.MicroOndas
    .OrderByDescending(ultimo => ultimo.CreatedAt)
    .FirstOrDefaultAsync();

    if (VerificaIExistenciaDoUltimo(ultimoMicroondas))
    {
        DateTime horarioAtual = DateTime.Now; 
        DateTime horarioSomado = ultimoMicroondas.CreatedAt.AddSeconds(ultimoMicroondas.Tempo); 

        if (VerificaHorario(horarioSomado, horarioAtual))
        {

            int valorRestanteEmSegundos = (int)(horarioAtual - ultimoMicroondas.CreatedAt).TotalSeconds;
            ultimoMicroondas.Tempo = valorRestanteEmSegundos + 30;

            await microOndasdb.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        if (VerificaExistenciaEEstado(ultimoMicroondas, microOndas))
        {
            ultimoMicroondas.Estado = EstadosMicroondasEnum.INICIADO;
            ultimoMicroondas.UpdateAt = DateTime.Now;

            await microOndasdb.SaveChangesAsync();
            return TypedResults.Ok("Aquecimento reiniciado");

        }

    }

    microOndas.CreatedAt = DateTime.Now;
    microOndas.Estado = EstadosMicroondasEnum.INICIADO;
    microOndasdb.MicroOndas.Add(microOndas);

    await microOndasdb.SaveChangesAsync();

    return TypedResults.Created($"/microondas/{microOndas.Id}", microOndas);
}

static async Task<IResult> CriaPrograma(Programas programa, MicroOndasDb microOndasdb)
{
    var caracterePrograma = await microOndasdb.Programas
    .Where(res => res.StgAquecimento == programa.StgAquecimento)
    .AnyAsync();


    if (programa.StgAquecimento == '.' || caracterePrograma)
        return TypedResults.BadRequest("Este caractere não pode ser usado");

    microOndasdb.Programas.Add(programa);

    await microOndasdb.SaveChangesAsync();

    return TypedResults.Created($"/microondas/{programa.Id}", programa);
}

static async Task<IResult> Aquecimento(int tempo = 30, int potencia = 10)
{

    if (VerificaIntervaloDeTempo(tempo))
        return TypedResults.BadRequest("O tempo deve ser entre 1 segundo e 2 minutos");

    if (VerificaIntervaloDePotencia(potencia))
        return TypedResults.BadRequest("A potencia deve ser entre 1 e 10");

    object resposta;

    if (VerificaIFormatacaoDeTempo(tempo))
    {
        int minutos = tempo / 60;
        int segundos = tempo % 60;

        resposta = new 
        {
            Tempo = $"{minutos}:{segundos:D2}",
            Potencia = potencia,
        };


        return TypedResults.Ok(resposta);
    }

    resposta = new
    {
        Tempo = $"{tempo}",
        Potencia = potencia,
    };

    return TypedResults.Ok(resposta);
}

static async Task<IResult> StatusAquecimento(int tempo, int potencia, string stgAquecimento = ".")
{
    string status = "";

    int vezes = (tempo * potencia);

    for (int i = 1; i <= vezes; i++)
    {
        if (i % potencia == 0 && i > 0)
        {
            status += $"{stgAquecimento} ";
        }
        else
        {
            status += $"{stgAquecimento}";
        }

    }

    status += " Aquecimento concluído";
    return TypedResults.Ok(status.ToString());
}

static async Task<IResult> PararPausarAquecimento(MicroOndasDb microOndasdb)
{
    var ultimoMicroondas = await microOndasdb.MicroOndas
    .Where(ultimo => ultimo.Estado != EstadosMicroondasEnum.CANCELADO)
    .OrderByDescending(ultimo => ultimo.CreatedAt)
    .FirstOrDefaultAsync();

    if (VerificaIExistenciaDoUltimo(ultimoMicroondas))
    {
        if(VerificaEstado(ultimoMicroondas))
        {
            ultimoMicroondas.Estado = EstadosMicroondasEnum.CANCELADO;
            ultimoMicroondas.UpdateAt = DateTime.Now;

            await microOndasdb.SaveChangesAsync();
            return TypedResults.Ok("Aquecimento cancelado");
        }

        ultimoMicroondas.Estado = EstadosMicroondasEnum.PAUSADO;
        ultimoMicroondas.UpdateAt = DateTime.Now;

        int diferenca = (int)(ultimoMicroondas.UpdateAt - ultimoMicroondas.CreatedAt).TotalSeconds;
        var tempoRestante = ultimoMicroondas.Tempo - (diferenca);

        ultimoMicroondas.Tempo = tempoRestante;
        await microOndasdb.SaveChangesAsync();

        return TypedResults.Ok("Aquecimento pausado");

    }

    return TypedResults.BadRequest("Não existe nenhum microondas em andamento");
}

static bool VerificaIntervaloDeTempo(int tempo)
{
    return tempo < 1 || tempo > 120;
}

static bool VerificaIntervaloDePotencia(int potencia)
{
    return potencia < 1 || potencia > 10;
}

static bool VerificaIFormatacaoDeTempo(int tempo)
{
    return tempo > 60 && tempo < 100;
}

static bool VerificaHorario(DateTime horarioSomado, DateTime horarioAtual)
{
    return horarioSomado > horarioAtual;
}

static bool VerificaExistenciaEEstado(MicroOndas ultimoMicroondas, MicroOndas microOndas)
{
    return ultimoMicroondas.CreatedAt == microOndas.CreatedAt && ultimoMicroondas.Estado != EstadosMicroondasEnum.CANCELADO;
}

static bool VerificaIExistenciaDoUltimo(MicroOndas ultimoMicroondas)
{
    return ultimoMicroondas != null;
}

static bool VerificaEstado(MicroOndas ultimoMicroondas)
{
    return ultimoMicroondas.Estado == EstadosMicroondasEnum.PAUSADO;
}

