using System.ComponentModel.DataAnnotations;

namespace GestãoCarros.Models
{
    public class Venda
    {
        public Guid VendaId { get; set; }
        public Guid VeiculoId { get; set; }
        public Veiculo? Veiculo { get; set; }
        public Guid ConcessionariaId { get; set; }
        public Concessionaria? Concessionaria { get; set; }

        [Required(ErrorMessage = "O Veículo é obrigatório.")]
        [Display(Name = "Selecione o Veículo")]
        public Guid UsuarioId { get; set; }

        [Required(ErrorMessage = "O Usuário é obrigatório.")]
        [Display(Name = "Selecione o Usuário")]
        public Usuario? Usuario { get; set; }

        [Required(ErrorMessage = "O valor da venda é obrigatório.")]
        [Display(Name = "Digite o Valor da Venda")]
        public decimal ValorVenda { get; set; }

        [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
        [Display(Name = "Digite o Nome do Cliente")]
        [MaxLength(150)]
        public string? ClienteNome { get; set; }

        [Required(ErrorMessage = "O CPF do cliente é obrigatório.")]
        [Display(Name = "Digite o CPF do Cliente")]
        [MaxLength(11)]
        public int ClienteCpf { get; set; }

        [Required(ErrorMessage = "A data da venda é obrigatória.")]
        [Display(Name = "Digite a Data da Venda")]
        public DateTime DataVenda { get; set; } = DateTime.Now;
    }
}