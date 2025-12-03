using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GestãoCarros.Models
{
    public class Usuario : IdentityUser<Guid>
    {
        [Required]
        public Guid ConcessionariaId { get; set; }
        public Concessionaria? Concessionaria { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [Display(Name = "Digite seu Nome")]
        [MaxLength(150, ErrorMessage = "O nome não pode passar de 150 caracteres.")]
        public string? Nome { get; set; }

        public Venda? Vendas { get; set; }

        public bool Ativo { get; set; } = true;
    }
}