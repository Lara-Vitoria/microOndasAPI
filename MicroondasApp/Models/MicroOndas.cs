using Microsoft.EntityFrameworkCore;

namespace MicroondasApp.Models
{
    [Keyless]
    public class MicroOndas
    {
        public int Tempo { get; set; }
        public int Potencia { get; set; }

        public MicroOndas(int tempo = 30, int potencia = 10)
        {
            this.Tempo = tempo;
            this.Potencia = potencia;
        }
    }
}
