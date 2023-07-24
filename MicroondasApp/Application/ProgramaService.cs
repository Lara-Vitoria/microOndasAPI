using MicroondasApp.Application.Contratos;
using MicroondasApp.Models;
using MicroondasApp.Persistence;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MicroondasApp.Service
{
    public class ProgramaService : IProgramaService
    {
        private readonly MicroOndasDb _contextProgramas;

        public ProgramaService(MicroOndasDb contextProgramas)
        {
            _contextProgramas = contextProgramas;
        }

        public async Task<Programas[]> GetProgramas()
        {
            try
            {
                var programas = await _contextProgramas.Programas.ToArrayAsync();

                if (programas == null) return null;

                return programas;
            }
            catch (Exception error)
            {
                throw new Exception(error.Message);
            }
        }

        public async Task<Programas> CriaPrograma(Programas programa)
        {

            try
            {
                var caracterePrograma = await _contextProgramas.Programas
                .Where(res => res.StgAquecimento == programa.StgAquecimento)
                .AnyAsync();


                if (programa.StgAquecimento == '.' || caracterePrograma)
                    throw new Exception("Este caractere não pode ser usado");

                _contextProgramas.Programas.Add(programa);

                await _contextProgramas.SaveChangesAsync();

                return programa;
            }
            catch (Exception error)
            {
                throw new Exception(error.Message);
            }

        }

        public async Task<Programas> GetProgramaById(int id)
        {
            try
            {
                var programa = await _contextProgramas.Programas.FindAsync(id);

                if (programa == null) return null;

                return programa;
            }
            catch (Exception error)
            {
                throw new Exception(error.Message);
            }
            
        }


    }
}
