namespace MicroondasApp.Application.Contratos
{
    public interface IStatusService
    {
        string StatusAquecimento(int tempo, int potencia, string stgAquecimento);
    }
}
