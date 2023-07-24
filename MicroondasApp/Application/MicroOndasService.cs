using MicroondasApp.Application.Contratos;
using MicroondasApp.Models;
using MicroondasApp.Persistence;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MicroondasApp.Service
{
    public class MicroOndasService : IMicroOndasService
    {
        private readonly MicroOndasDb _contextMicroondas;
        

        public MicroOndasService(MicroOndasDb contextMicroondas, MicroOndasDb contextProgramas)
        {
            _contextMicroondas = contextMicroondas;
        }

        public async Task<MicroOndas> Aquecimento(int tempo = 30, int potencia = 10)
        {
            if (VerificaIntervaloDeTempo(tempo))
                throw new Exception("O tempo deve ser entre 1 segundo e 2 minutos");

            if (VerificaIntervaloDePotencia(potencia))
                throw new Exception("A potencia deve ser entre 1 e 10");

            try
            {
                var microondas = new MicroOndas();

                if (VerificaIFormatacaoDeTempo(tempo))
                {
                    tempo = tempo / 60;

                    microondas = new MicroOndas
                    {
                        Tempo = tempo,
                        Potencia = potencia,
                        Estado = EstadosMicroondasEnum.INICIADO,
                        CreatedAt = DateTime.Now,
                        UpdateAt = DateTime.Now
                    };

                    _contextMicroondas.MicroOndas.Add(microondas);
                    await _contextMicroondas.SaveChangesAsync();

                    return microondas;

                }

                microondas = new MicroOndas
                {
                    Tempo = tempo,
                    Potencia = potencia,
                    Estado = EstadosMicroondasEnum.INICIADO,
                    CreatedAt = DateTime.Now,
                    UpdateAt = DateTime.Now
                };

                return microondas;
            }
            catch (Exception error)
            {

                throw new Exception(error.Message);
            }

           
        }

        public async Task<MicroOndas> CriaMicroOndas(MicroOndas microOndas)
        {

            if (VerificaIntervaloDeTempo(microOndas.Tempo))
                throw new Exception("O tempo deve ser entre 1 segundo e 2 minutos");

            if (VerificaIntervaloDePotencia(microOndas.Potencia))
                throw new Exception("A potencia deve ser entre 1 e 10");


            try
            {
                var ultimoMicroondasTask = _contextMicroondas.MicroOndas
                        .OrderByDescending(ultimo => ultimo.CreatedAt)
                        .FirstOrDefaultAsync();

                var ultimoMicroondas = ultimoMicroondasTask.Result;

                if (VerificaIExistenciaDoUltimo(ultimoMicroondas))
                {
                    DateTime horarioAtual = DateTime.Now;
                    DateTime horarioSomado = ultimoMicroondas.CreatedAt.AddSeconds(ultimoMicroondas.Tempo);

                    if (VerificaHorario(horarioSomado, horarioAtual))
                    {

                        int valorRestanteEmSegundos = (int)(horarioAtual - ultimoMicroondas.CreatedAt).TotalSeconds;
                        ultimoMicroondas.Tempo = valorRestanteEmSegundos + 30;

                        await _contextMicroondas.SaveChangesAsync();
                        return null;
                    }

                    if (VerificaExistenciaEEstado(ultimoMicroondas, microOndas))
                    {
                        ultimoMicroondas.Estado = EstadosMicroondasEnum.INICIADO;
                        ultimoMicroondas.UpdateAt = DateTime.Now;

                        await _contextMicroondas.SaveChangesAsync();
                        return null;

                    }

                }

                microOndas.CreatedAt = DateTime.Now;
                microOndas.Estado = EstadosMicroondasEnum.INICIADO;
                _contextMicroondas.Add(microOndas);

                await _contextMicroondas.SaveChangesAsync();

                return microOndas;
            }
            catch (Exception error)
            {

                throw new Exception(error.Message);
            }
            
        }

        public async Task<MicroOndas[]> GetMicroOndas()
        {
            try
            {
                var microondas = await _contextMicroondas.MicroOndas.ToArrayAsync();

                if (microondas == null) return null;

                return microondas;
            }
            catch (Exception error)
            {
                throw new Exception(error.Message);
            }

        }

        public MicroOndas PararPausarAquecimento()
        {
            var ultimoMicroondasTask = _contextMicroondas.MicroOndas
            .Where(ultimo => ultimo.Estado != EstadosMicroondasEnum.CANCELADO)
            .OrderByDescending(ultimo => ultimo.CreatedAt)
            .FirstOrDefaultAsync();

            var ultimoMicroondas = ultimoMicroondasTask.Result;

            if (VerificaIExistenciaDoUltimo(ultimoMicroondas))
            {
                if (VerificaEstado(ultimoMicroondas))
                {
                    ultimoMicroondas.Estado = EstadosMicroondasEnum.CANCELADO;
                    ultimoMicroondas.UpdateAt = DateTime.Now;

                    _contextMicroondas.SaveChangesAsync();
                    return null;
                }

                ultimoMicroondas.Estado = EstadosMicroondasEnum.PAUSADO;
                ultimoMicroondas.UpdateAt = DateTime.Now;

                int diferenca = (int)(ultimoMicroondas.UpdateAt - ultimoMicroondas.CreatedAt).TotalSeconds;
                var tempoRestante = ultimoMicroondas.Tempo - (diferenca);

                ultimoMicroondas.Tempo = tempoRestante;
                _contextMicroondas.SaveChangesAsync();

                return null;

            }

            throw new Exception("Não existe nenhum microondas em andamento");
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
    }
}
