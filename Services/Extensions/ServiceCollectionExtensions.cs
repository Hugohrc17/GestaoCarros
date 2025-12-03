using GestãoCarros.Services.Fabricantes;
using GestãoCarros.Services.Veiculo;
using GestãoCarros.Services.Venda;
using GestaoCarros.Services.Repo;
using ImobiFlow.Api.Core.Interfaces;
using GestãoCarros.Services.Usuarios;

namespace GestãoCarros.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<FabricanteService>();
            services.AddScoped<UsuarioService>();
            services.AddScoped<VeiculoService>();
            services.AddScoped<VendaService>();
            services.AddScoped<IRepositorioGenerico, RepositorioGenerico>();
            return services;
        }
    }
}
