namespace GestãoCarros.Models
{
    public class Concessionaria
    {
        public Guid ConcessionariaId { get; set; }
        public string? Nome { get; set; }

        public string? Rua { get; set; }

        public string? Cidade { get; set; }

        public string? Estado { get; set; }

        public string? CEP { get; set; }

        public ICollection<Veiculo>? Veiculos { get; set; }

        public ICollection<Venda>? Vendas { get; set; }

        public ICollection<Usuario>? Usuarios { get; set; }

        public ICollection<Relatorio>? Relatorios { get; set; }

        public bool Ativo { get; set; } = true;
    }
}