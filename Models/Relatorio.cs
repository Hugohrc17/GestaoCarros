using System.ComponentModel.DataAnnotations;

namespace GestãoCarros.Models
{
    public class Relatorio
    {
        [Key]
        [Required]
        public Guid RelatorioId { get; set; }
    }
}