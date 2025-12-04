using AutoMapper;
using GestãoCarros.Models;
using GestãoCarros.Models.Dtos;
using ImobiFlow.Api.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace GestãoCarros.Services.Concessionaria
{
    public class ConcessionariaService
    {
        private readonly IRepositorioGenerico _repositorioGenerico;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public ConcessionariaService(IRepositorioGenerico repositorioGenerico, IMapper mapper, IMemoryCache cache)
        {
            _repositorioGenerico = repositorioGenerico;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Models.Concessionaria?> ObterPorIdAsync(Guid concessionariaId)
        {
            var concessionaria = await _repositorioGenerico.ObterPorIdAsync<Models.Concessionaria>(concessionariaId)
                ?? throw new Exception("Concessionária não encontrada, tente novamente");

            return concessionaria;
        }

        public async Task<IEnumerable<ConcessionariaDto>> ObterTodosAsync()
        {
            string cacheConcessionarias = "Concessionarias";

            if (_cache.TryGetValue(cacheConcessionarias, out var concessionariasCache) && concessionariasCache is IEnumerable<ConcessionariaDto> Concessionarias)
            {
                return Concessionarias;
            }

            var TodasConcessionarias = await _repositorioGenerico.ObterTodosAsync<Models.Concessionaria>(c => true)
                ?? throw new Exception("Erro ao obter todas concessionárias");

            var ConcessionariasDto = _mapper.Map<IEnumerable<ConcessionariaDto>>(TodasConcessionarias);

            _cache.Set(cacheConcessionarias, ConcessionariasDto, TimeSpan.FromMinutes(5));

            return ConcessionariasDto;
        }

        public async Task<Models.Concessionaria> AdicionarAsync(ConcessionariaDto concessionariaDto)
        {
            var concessionaria = _mapper.Map<Models.Concessionaria>(concessionariaDto);
            var resultado = await _repositorioGenerico.AdicionarAsync(concessionaria)
                ?? throw new Exception("Erro ao adicionar a Concessionária, por favor tente novamente");

            _cache.Remove("Concessionarias");

            return resultado;
        }

        public async Task AtualizarAsync(Guid id, Models.Concessionaria concessionaria)
        {
            var concessionariaExistente = await _repositorioGenerico.ObterPorIdAsync<Models.Concessionaria>(id)
                ?? throw new Exception("Concessionária não encontrada");

            await _repositorioGenerico.AtualizarAsync(concessionaria);
            _cache.Remove("Concessionarias");
        }

        public async Task ExcluirAsync(Guid concessionariaId)
        {
            await _repositorioGenerico.ExcluirAsync<Models.Concessionaria>(concessionariaId);
            _cache.Remove("Concessionarias");
        }
    }
}
