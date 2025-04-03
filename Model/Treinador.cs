using System.ComponentModel.DataAnnotations;

namespace APITATT1.Model
{
    public class Treinador
    {
        [Key]
        public int id { get; set; }
        [Required]
        [StringLength(35)]
        public string Nome { get; set; }
        [Required]

        public int idade { get; set; }
        [Required]
        public int telefone { get; set; }
        [Required]
        public string Endereco { get; set; }

    }
}
