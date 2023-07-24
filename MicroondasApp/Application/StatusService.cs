using MicroondasApp.Application.Contratos;

namespace MicroondasApp.Application
{
    public class StatusService : IStatusService
    {
        public string StatusAquecimento(int tempo, int potencia, string stgAquecimento)
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
            return status.ToString();
        }
    }
}
