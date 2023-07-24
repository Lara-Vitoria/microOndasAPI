using MicroondasApp.Models;

namespace MicroondasApp.Application.Contratos
{
    public interface IProgramaService
    {
        Task<Programas[]> GetProgramas();
        Task<Programas> GetProgramaById(int id);
        Task<Programas> CriaPrograma(Programas programa);

    }
}
