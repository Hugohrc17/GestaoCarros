using AutoMapper;
using GestãoCarros.Models;
using GestãoCarros.Models.Dtos;
using ImobiFlow.Api.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace GestãoCarros.Services.Veiculo
{
    public class VeiculoService
    {
        private readonly IRepositorioGenerico _repositorioGenerico;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public VeiculoService(IRepositorioGenerico repositorioGenerico, IMapper mapper, IMemoryCache cache)
        {
            _repositorioGenerico = repositorioGenerico;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Models.Veiculo?> ObterPorIdAsync(Guid veiculoId)
        {
            var veiculo = await _repositorioGenerico.ObterPorIdAsync<Models.Veiculo>(veiculoId)
                ?? throw new Exception("Veículo não encontrado, tente novamente");

            return veiculo;
        }

        public async Task<IEnumerable<VeiculoDto>> ObterTodosAsync()
        {
            string cacheVeiculos = "Veiculos";

            if (_cache.TryGetValue(cacheVeiculos, out var veiculosCache) && veiculosCache is IEnumerable<VeiculoDto> Veiculos)
            {
                return Veiculos;
            }

            var TodosVeiculos = await _repositorioGenerico.ObterTodosAsync<Models.Veiculo>(v => true)
                ?? throw new Exception("Erro ao obter todos veículos");

            var VeiculosDto = _mapper.Map<IEnumerable<VeiculoDto>>(TodosVeiculos);

            _cache.Set(cacheVeiculos, VeiculosDto, TimeSpan.FromMinutes(5));

            return VeiculosDto;
        }

        public async Task<IEnumerable<VeiculoDto>> ObterPorConcessionariaAsync(Guid concessionariaId)
        {
            var veiculos = await _repositorioGenerico.ObterTodosAsync<Models.Veiculo>(v => v.ConcessionariaId == concessionariaId)
                ?? throw new Exception("Erro ao obter veículos da concessionária");

            var veiculosDto = _mapper.Map<IEnumerable<VeiculoDto>>(veiculos);

            return veiculosDto;
        }

        public async Task<Models.Veiculo> AdicionarAsync(VeiculoDto veiculoDto, Guid concessionariaId)
        {
            // Validar se concessionária existe
            var concessionaria = await _repositorioGenerico.ObterPorIdAsync<Models.Concessionaria>(concessionariaId);
            if (concessionaria == null)
                throw new Exception("Concessionária não encontrada");

            // Validar se fabricante existe
            var fabricante = await _repositorioGenerico.ObterPorIdAsync<Fabricante>(veiculoDto.Fabricante?.FabricanteId ?? Guid.Empty);
            if (fabricante == null)
                throw new Exception("Fabricante não encontrado");

            var veiculo = _mapper.Map<Models.Veiculo>(veiculoDto);
            veiculo.ConcessionariaId = concessionariaId;
            veiculo.VeiculoId = Guid.NewGuid();

            var resultado = await _repositorioGenerico.AdicionarAsync(veiculo)
                ?? throw new Exception("Erro ao adicionar o Veículo, por favor tente novamente");

            _cache.Remove("Veiculos");

            return resultado;
        }

        public async Task AtualizarAsync(Guid id, Models.Veiculo veiculo)
        {
            var veiculoExistente = await _repositorioGenerico.ObterPorIdAsync<Models.Veiculo>(id)
                ?? throw new Exception("Veículo não encontrado");

            await _repositorioGenerico.AtualizarAsync(veiculo);
            _cache.Remove("Veiculos");
        }

        public async Task ExcluirAsync(Guid veiculoId)
        {
            await _repositorioGenerico.ExcluirAsync<Models.Veiculo>(veiculoId);
            _cache.Remove("Veiculos");
        }
    }
}
