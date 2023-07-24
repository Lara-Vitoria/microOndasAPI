using MicroondasApp.Application.Contratos;
using MicroondasApp.Models;
using MicroondasApp.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MicroondasApp.Controller
{
    [Route("/[controller]")]
    [ApiController]
    public class MicroOndasController : ControllerBase
    {

        private readonly MicroOndasDb _contextMicroondas;

        private readonly IMicroOndasService _microOndasService;
        private readonly IStatusService _statusService;

        public MicroOndasController(MicroOndasDb contextMicroondas, IStatusService statusService, IMicroOndasService microOndasService)
        {
            _contextMicroondas = contextMicroondas;
            _microOndasService = microOndasService;
            _statusService = statusService;

        }

        [HttpGet("/microondas")]
        public async Task<ActionResult> GetMicroOndas()
        {
            try
            {
                var microondas = await _microOndasService.GetMicroOndas();
                if (microondas == null) return NoContent();

                return Ok(microondas);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao tentar recuperar microondas. Erro: {ex.Message}");
            }


        }

        [HttpPost("/microondas/")]
        public async Task<ActionResult> CriaMicroOndas(MicroOndas microOndas)
        {
            try
            {
                var microondas = await _microOndasService.CriaMicroOndas(microOndas);

                if (microondas == null) return NoContent();

                return Ok(microondas);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao tentar adicionar microondas. Erro:{ex.Message}");
            }
        }


        [HttpGet("/microondas/aquecimento/{tempo?}/{potencia?}")]
        public async Task<ActionResult> Aquecimento(int tempo = 30, int potencia = 10)
        {

            try
            {
                var microondas = await _microOndasService.Aquecimento(tempo, potencia);
                if (microondas == null) return NoContent();

                return Ok(microondas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/microondas/statusAquecimento/{tempo?}/{potencia?}/{stgAquecimento?}")]
        public async Task<ActionResult> StatusAquecimento(int tempo, int potencia, string stgAquecimento = ".")
        {
            try
            {
                var status = _statusService.StatusAquecimento(tempo, potencia, stgAquecimento);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/microondas/ParaPausar")]
        public async Task<ActionResult> PararPausarAquecimento()
        {
            try
            {
                _microOndasService.PararPausarAquecimento();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
