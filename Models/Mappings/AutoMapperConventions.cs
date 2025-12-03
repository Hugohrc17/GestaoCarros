using AutoMapper;
using GestãoCarros.Models;
using GestãoCarros.Models.Dtos;

namespace Mappings
{
    public class AutoMapperConventions : Profile
    {
        public AutoMapperConventions()
        {
            CreateMap<Fabricante, FabricanteDto>().ReverseMap();

            CreateMap<Veiculo, VeiculoDto>().ReverseMap();

            CreateMap<Usuario, UsuarioDto>().ReverseMap();

            CreateMap<Venda, VendaDto>().ReverseMap();

            CreateMap<Concessionaria, ConcessionariaDto>().ReverseMap();
        }
    }
}
