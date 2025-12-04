using AutoMapper;
using GestãoCarros.Models;
using GestãoCarros.Models.Dtos;
using GestãoCarros.Services.Fabricantes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace GestãoCarros.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FabricanteController : ControllerBase
    {
        private readonly FabricanteService _fabricanteService;
        private readonly IMapper _mapper;

        public FabricanteController(FabricanteService fabricanteService, IMapper mapper)
        {
            _fabricanteService = fabricanteService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,AdminInterno")]
        public async Task<IActionResult> ObterTodos()
        {
            var fabricante = await _fabricanteService.ObterTodosAsync();
            return Ok(fabricante);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var fabricante = await _fabricanteService.ObterPorIdAsync(id);
            if (fabricante == null)
            {
                return NotFound();
            }

            return Ok(fabricante);
        }

        [HttpPost]
        public async Task<IActionResult> Adicionar(FabricanteDto fabricanteDto)
        {
            var fabricante = await _fabricanteService.AdicionarAsync(fabricanteDto);
            if (fabricante == null)
            {
                return BadRequest("Não foi possível adicionar o fabricante.");
            }

            return Ok(fabricante);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(Guid id, FabricanteDto fabricanteDto)
        {
            var fabricante = await _fabricanteService.ObterPorIdAsync(id);
            if (fabricante == null)
            {
                return NotFound();
            }

            fabricanteDto.FabricanteId = id; 
            var fabricanteAtualizado = _mapper.Map(fabricanteDto, fabricante);
            await _fabricanteService.AtualizarAsync(id, fabricanteAtualizado);
            return Ok(fabricanteAtualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(Guid id)
        {
            var fabricante = await _fabricanteService.ObterPorIdAsync(id);
            if (fabricante == null)
            {
                return NotFound();
            }

            await _fabricanteService.ExcluirAsync(id);
            return NoContent();
        }
    }
}