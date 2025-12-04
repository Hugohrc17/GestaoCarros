using AutoMapper;
using GestãoCarros.Models;
using GestãoCarros.Models.Dtos;
using GestãoCarros.Services.Venda;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestãoCarros.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VendaController : ControllerBase
    {
        private readonly VendaService _vendaService;
        private readonly IMapper _mapper;

        public VendaController(VendaService vendaService, IMapper mapper)
        {
            _vendaService = vendaService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,AdminInterno")]
        public async Task<IActionResult> ObterTodos()
        {
            var vendas = await _vendaService.ObterTodosAsync();
            return Ok(vendas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var venda = await _vendaService.ObterPorIdAsync(id);
            if (venda == null)
            {
                return NotFound();
            }

            var vendaDto = _mapper.Map<VendaDto>(venda);
            return Ok(vendaDto);
        }

        [HttpGet("concessionaria/{concessionariaId}")]
        [Authorize(Roles = "Admin,AdminInterno")]
        public async Task<IActionResult> ObterPorConcessionaria(Guid concessionariaId)
        {
            var vendas = await _vendaService.ObterPorConcessionariaAsync(concessionariaId);
            return Ok(vendas);
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> ObterPorUsuario(Guid usuarioId)
        {
            var vendas = await _vendaService.ObterPorUsuarioAsync(usuarioId);
            return Ok(vendas);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,AdminInterno,Vendedor")]
        public async Task<IActionResult> Adicionar([FromBody] VendaDto vendaDto)
        {
            var concessionariaId = User.FindFirst("ConcessionariaId")?.Value;
            if (string.IsNullOrEmpty(concessionariaId))
            {
                return Unauthorized("ConcessionariaId não encontrada no token.");
            }

            var venda = await _vendaService.AdicionarAsync(vendaDto, Guid.Parse(concessionariaId));
            if (venda == null)
            {
                return BadRequest("Não foi possível adicionar a venda.");
            }

            return Ok(venda);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,AdminInterno")]
        public async Task<IActionResult> Atualizar(Guid id, VendaDto vendaDto)
        {
            var venda = await _vendaService.ObterPorIdAsync(id);
            if (venda == null)
            {
                return NotFound();
            }

            var vendaAtualizada = _mapper.Map(vendaDto, venda);
            await _vendaService.AtualizarAsync(id, vendaAtualizada);
            return Ok(vendaAtualizada);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,AdminInterno")]
        public async Task<IActionResult> Excluir(Guid id)
        {
            var venda = await _vendaService.ObterPorIdAsync(id);
            if (venda == null)
            {
                return NotFound();
            }

            await _vendaService.ExcluirAsync(id);
            return NoContent();
        }
    }
}