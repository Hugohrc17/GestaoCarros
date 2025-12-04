using AutoMapper;
using GestãoCarros.Models;
using GestãoCarros.Models.Dtos;
using GestãoCarros.Services.Veiculo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestãoCarros.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class VeiculoController : ControllerBase
    {
        private readonly VeiculoService _veiculoService;
        private readonly IMapper _mapper;

        public VeiculoController(VeiculoService veiculoService, IMapper mapper)
        {
            _veiculoService = veiculoService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,AdminInterno")]
        public async Task<IActionResult> ObterTodos()
        {
            var veiculos = await _veiculoService.ObterTodosAsync();
            return Ok(veiculos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var veiculo = await _veiculoService.ObterPorIdAsync(id);
            if (veiculo == null)
            {
                return NotFound();
            }

            var veiculoDto = _mapper.Map<VeiculoDto>(veiculo);
            return Ok(veiculoDto);
        }

        [HttpGet("concessionaria/{concessionariaId}")]
        public async Task<IActionResult> ObterPorConcessionaria(Guid concessionariaId)
        {
            var veiculos = await _veiculoService.ObterPorConcessionariaAsync(concessionariaId);
            return Ok(veiculos);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,AdminInterno")]
        public async Task<IActionResult> Adicionar([FromBody] VeiculoDto veiculoDto)
        {
            var concessionariaId = User.FindFirst("ConcessionariaId")?.Value;
            if (string.IsNullOrEmpty(concessionariaId))
            {
                return Unauthorized("ConcessionariaId não encontrada no token.");
            }

            var veiculo = await _veiculoService.AdicionarAsync(veiculoDto, Guid.Parse(concessionariaId));
            if (veiculo == null)
            {
                return BadRequest("Não foi possível adicionar o veículo.");
            }

            return Ok(veiculo);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,AdminInterno")]
        public async Task<IActionResult> Atualizar(Guid id, VeiculoDto veiculoDto)
        {
            var veiculo = await _veiculoService.ObterPorIdAsync(id);
            if (veiculo == null)
            {
                return NotFound();
            }

            var veiculoAtualizado = _mapper.Map(veiculoDto, veiculo);
            await _veiculoService.AtualizarAsync(id, veiculoAtualizado);
            return Ok(veiculoAtualizado);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,AdminInterno")]
        public async Task<IActionResult> Excluir(Guid id)
        {
            var veiculo = await _veiculoService.ObterPorIdAsync(id);
            if (veiculo == null)
            {
                return NotFound();
            }

            await _veiculoService.ExcluirAsync(id);
            return NoContent();
        }
    }
}