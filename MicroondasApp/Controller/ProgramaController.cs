using MicroondasApp.Application.Contratos;
using MicroondasApp.Models;
using MicroondasApp.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace MicroondasApp.Controller
{
    [Route("/[controller]")]
    [ApiController]
    public class ProgramaController : ControllerBase
    {
        private readonly MicroOndasDb _contextProgramas;
        private readonly IProgramaService _programaService;
        private readonly IStatusService _statusService;
        public ProgramaController( MicroOndasDb contextProgramas, IProgramaService programaService, IStatusService statusService)
        {
            _contextProgramas = contextProgramas;
            _programaService = programaService;
            _statusService = statusService;
        }

        [HttpGet("/programa")]
        public async Task<ActionResult> GetProgramas()
        {
            try
            {
                var programas = await _programaService.GetProgramas();
                if (programas == null) return NoContent();

                return Ok(programas);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao tentar recuperar os programas. Erro: {ex.Message}");
            }

        }

        [HttpGet("/programa/{id}")]
        public async Task<ActionResult> GetProgramaById(int id)
        {

            try
            {
                var programa = await _programaService.GetProgramaById(id);

                if (programa == null) return NoContent();

                return Ok(programa);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/programa/statusAquecimento/{tempo?}/{potencia?}/{stgAquecimento?}")]
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

        [HttpPost("/programa")]
        public async Task<ActionResult> CriaPrograma(Programas model)
        {
            try
            {

                var programa = await _programaService.CriaPrograma(model);

                if (programa == null) return NoContent();


                return Ok(programa);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao tentar adicionar um programa. Erro:{ex.Message}");
            }
        }
    }
}
