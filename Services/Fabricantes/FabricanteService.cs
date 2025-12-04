using System;
using AutoMapper;
using GestãoCarros.Models;
using GestãoCarros.Models.Dtos;
using ImobiFlow.Api.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GestãoCarros.Services.Fabricantes
{
    public class FabricanteService
    {
        private readonly IRepositorioGenerico _repositorioGenerico;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public FabricanteService(IRepositorioGenerico repositorioGenerico, IMapper mapper, IMemoryCache cache)
        {
            _repositorioGenerico = repositorioGenerico;
            _mapper = mapper;
            _cache = cache;

        }

        public async Task<Fabricante?> ObterPorIdAsync(Guid fabricanteId)
        {
            var Fabricante = await _repositorioGenerico.ObterPorIdAsync<Fabricante>(fabricanteId)
             ?? throw new Exception("Fabricante não encontrado, tente novamente");

            return Fabricante;
        }
        public async Task<IEnumerable<FabricanteDto>> ObterTodosAsync()
        {
            string cacheFabricantes = $"Fabricantes";

            if (_cache.TryGetValue(cacheFabricantes, out var fabricantesCache) && fabricantesCache is IEnumerable<FabricanteDto> Fabricantes)
            {
                return Fabricantes;
            }

            var TodosFabricantes = await _repositorioGenerico.ObterTodosAsync<Fabricante>(f => true)
             ?? throw new Exception("Erro ao obter todos fabricantes");

            var FabricantesDto = _mapper.Map<IEnumerable<FabricanteDto>>(TodosFabricantes);

            _cache.Set(cacheFabricantes, FabricantesDto, TimeSpan.FromMinutes(5));

            return FabricantesDto;
        }

        public async Task<Fabricante> AdicionarAsync(FabricanteDto fabricante)
        {
            fabricante.FabricanteId = Guid.NewGuid();
            fabricante.Ativo = true;
            var fabric = _mapper.Map<Fabricante>(fabricante);
            var Sucesso = await _repositorioGenerico.AdicionarAsync(fabric)
                ?? throw new Exception("Erro ao adicionar o Fabricante, por favor tente novamente");
            _cache.Remove("Fabricantes");

            return Sucesso;
        }

        public async Task AtualizarAsync(Guid id, Fabricante fabricante)
        {
            var fabricant = await _repositorioGenerico.ObterPorIdAsync<Fabricante>(id)
                 ?? throw new Exception("Fabricante não encontrado");

            await _repositorioGenerico.AtualizarAsync(fabricante);
            _cache.Remove("Fabricantes");
        }

        public async Task ExcluirAsync(Guid fabricanteId)
        {
            await _repositorioGenerico.ExcluirAsync<Fabricante>(fabricanteId);
        }
    }
}