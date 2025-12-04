using AutoMapper;
using GestãoCarros.Models;
using GestãoCarros.Models.Dtos;
using ImobiFlow.Api.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace GestãoCarros.Services.Venda
{
    public class VendaService
    {
        private readonly IRepositorioGenerico _repositorioGenerico;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public VendaService(IRepositorioGenerico repositorioGenerico, IMapper mapper, IMemoryCache cache)
        {
            _repositorioGenerico = repositorioGenerico;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<Models.Venda?> ObterPorIdAsync(Guid vendaId)
        {
            var venda = await _repositorioGenerico.ObterPorIdAsync<Models.Venda>(vendaId)
                ?? throw new Exception("Venda não encontrada, tente novamente");

            return venda;
        }

        public async Task<IEnumerable<VendaDto>> ObterTodosAsync()
        {
            string cacheVendas = "Vendas";

            if (_cache.TryGetValue(cacheVendas, out var vendasCache) && vendasCache is IEnumerable<VendaDto> Vendas)
            {
                return Vendas;
            }

            var TodasVendas = await _repositorioGenerico.ObterTodosAsync<Models.Venda>(v => true)
                ?? throw new Exception("Erro ao obter todas vendas");

            var VendasDto = _mapper.Map<IEnumerable<VendaDto>>(TodasVendas);

            _cache.Set(cacheVendas, VendasDto, TimeSpan.FromMinutes(5));

            return VendasDto;
        }

        public async Task<IEnumerable<VendaDto>> ObterPorConcessionariaAsync(Guid concessionariaId)
        {
            var vendas = await _repositorioGenerico.ObterTodosAsync<Models.Venda>(v => v.ConcessionariaId == concessionariaId)
                ?? throw new Exception("Erro ao obter vendas da concessionária");

            var vendasDto = _mapper.Map<IEnumerable<VendaDto>>(vendas);

            return vendasDto;
        }

        public async Task<IEnumerable<VendaDto>> ObterPorUsuarioAsync(Guid usuarioId)
        {
            var vendas = await _repositorioGenerico.ObterTodosAsync<Models.Venda>(v => v.UsuarioId == usuarioId)
                ?? throw new Exception("Erro ao obter vendas do usuário");

            var vendasDto = _mapper.Map<IEnumerable<VendaDto>>(vendas);

            return vendasDto;
        }

        public async Task<Models.Venda> AdicionarAsync(VendaDto vendaDto, Guid concessionariaId)
        {
            // Validar se concessionária existe
            var concessionaria = await _repositorioGenerico.ObterPorIdAsync<Models.Concessionaria>(concessionariaId);
            if (concessionaria == null)
                throw new Exception("Concessionária não encontrada");

            // Validar se veículo existe
            var veiculo = await _repositorioGenerico.ObterPorIdAsync<Models.Veiculo>(vendaDto.Veiculo?.VeiculoId ?? Guid.Empty);
            if (veiculo == null)
                throw new Exception("Veículo não encontrado");

            // Validar se usuário existe
            var usuario = await _repositorioGenerico.ObterPorIdAsync<Usuario>(vendaDto.Usuario?.Id ?? Guid.Empty);
            if (usuario == null)
                throw new Exception("Usuário não encontrado");

            var venda = _mapper.Map<Models.Venda>(vendaDto);
            venda.ConcessionariaId = concessionariaId;
            venda.VendaId = Guid.NewGuid();
            venda.DataVenda = DateTime.Now;

            var resultado = await _repositorioGenerico.AdicionarAsync(venda)
                ?? throw new Exception("Erro ao adicionar a Venda, por favor tente novamente");

            _cache.Remove("Vendas");

            return resultado;
        }

        public async Task AtualizarAsync(Guid id, Models.Venda venda)
        {
            var vendaExistente = await _repositorioGenerico.ObterPorIdAsync<Models.Venda>(id)
                ?? throw new Exception("Venda não encontrada");

            await _repositorioGenerico.AtualizarAsync(venda);
            _cache.Remove("Vendas");
        }

        public async Task ExcluirAsync(Guid vendaId)
        {
            await _repositorioGenerico.ExcluirAsync<Models.Venda>(vendaId);
            _cache.Remove("Vendas");
        }
    }
}
