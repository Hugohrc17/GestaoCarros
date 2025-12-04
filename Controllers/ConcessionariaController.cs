using AutoMapper;
using GestãoCarros.Models;
using GestãoCarros.Models.Dtos;
using GestãoCarros.Services.Concessionaria;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestãoCarros.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class ConcessionariaController : ControllerBase
    {
        private readonly ConcessionariaService _concessionariaService;
        private readonly IMapper _mapper;

        public ConcessionariaController(ConcessionariaService concessionariaService, IMapper mapper)
        {
            _concessionariaService = concessionariaService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,AdminInterno")]
        public async Task<IActionResult> ObterTodos()
        {
            var concessionarias = await _concessionariaService.ObterTodosAsync();
            return Ok(concessionarias);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var concessionaria = await _concessionariaService.ObterPorIdAsync(id);
            if (concessionaria == null)
            {
                return NotFound();
            }

            var concessionariaDto = _mapper.Map<ConcessionariaDto>(concessionaria);
            return Ok(concessionariaDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Adicionar(ConcessionariaDto concessionariaDto)
        {
            var concessionaria = await _concessionariaService.AdicionarAsync(concessionariaDto);
            if (concessionaria == null)
            {
                return BadRequest("Não foi possível adicionar a concessionária.");
            }

            return Ok(concessionaria);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,AdminInterno")]
        public async Task<IActionResult> Atualizar(Guid id, ConcessionariaDto concessionariaDto)
        {
            var concessionaria = await _concessionariaService.ObterPorIdAsync(id);
            if (concessionaria == null)
            {
                return NotFound();
            }

            var concessionariaAtualizada = _mapper.Map(concessionariaDto, concessionaria);
            await _concessionariaService.AtualizarAsync(id, concessionariaAtualizada);
            return Ok(concessionariaAtualizada);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Excluir(Guid id)
        {
            var concessionaria = await _concessionariaService.ObterPorIdAsync(id);
            if (concessionaria == null)
            {
                return NotFound();
            }

            await _concessionariaService.ExcluirAsync(id);
            return NoContent();
        }
    }
}