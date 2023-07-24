using MicroondasApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MicroondasApp.Application.Contratos
{
    public interface IMicroOndasService
    {
        Task<MicroOndas[]> GetMicroOndas();
        Task<MicroOndas> CriaMicroOndas(MicroOndas microOndas);
        MicroOndas PararPausarAquecimento();

        Task<MicroOndas> Aquecimento(int tempo, int potencia);
        

    }
}
