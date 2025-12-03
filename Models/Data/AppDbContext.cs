using GestãoCarros.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestãoCarros.Data
{
    public class AppDbContext : IdentityDbContext<Usuario, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options) {}

        public DbSet<Fabricante> Fabricantes { get; set; }

        public DbSet<Veiculo> Veiculos { get; set; }

        public DbSet<Venda> Vendas { get; set; }
        public DbSet<Concessionaria> Concessionarias { get; set; }
    }
}