using Microsoft.EntityFrameworkCore;

namespace MicroondasApp.Models
{
    public class MicroOndas
    {
        public int Id { get; set; }
        public int Tempo { get; set; }
        public int Potencia { get; set; }
        public EstadosMicroondasEnum Estado { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public MicroOndas(int tempo = 30, int potencia = 10)
        {
            this.Tempo = tempo;
            this.Potencia = potencia;
        }

    }
}
