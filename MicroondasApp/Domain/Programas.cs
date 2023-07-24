namespace MicroondasApp.Models
{
    public class Programas
    {
        public int Id { get; set; }
        public int Tempo { get; set; }
        public int Potencia { get; set; }
        public string Nome { get; set; }
        public string Alimento { get; set; }
        public char StgAquecimento { get; set; }
        public string ?Instrucoes { get; set; }
    }
}
