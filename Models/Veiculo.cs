using System.ComponentModel.DataAnnotations;
using GestãoCarros.Models.Enums;


namespace GestãoCarros.Models
{
    public class Veiculo
    {
        [Key]
        public Guid VeiculoId { get; set; }
        public Guid ConcessionariaId { get; set; }
        public Concessionaria? Concessionaria { get; set; }

        [Required(ErrorMessage = "O modelo é obrigatório.")]
        [Display(Name = "Digite o Modelo")]
        [MaxLength(100)]
        public string? Modelo { get; set; }
        
        [Required(ErrorMessage = "O ano de fabricação é obrigatório.")]
        [Display(Name = "Digite o Ano de Fabricação")]
        [MaxLength(4)]
        public int AnoFabricacao { get; set; }
        
        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Display(Name = "Digite o Preço")]
        public decimal Preco { get; set; }

        public Guid FabricanteId { get; set; }

        [Required(ErrorMessage = "O fabricante é obrigatório.")]
        [Display(Name = "Selecione o Fabricante")]
        public Fabricante? Fabricante { get; set; }

        [Required(ErrorMessage = "O tipo do veículo é obrigatório.")]
        [Display(Name = "Selecione o Tipo do Veículo")]
        public TipoVeiculo? Tipo { get; set; }

        [MaxLength(500)]
        public string? Descricao { get; set; }

        public bool Ativo { get; set; } = true;
    }
}