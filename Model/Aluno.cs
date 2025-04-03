using System.ComponentModel.DataAnnotations;

namespace APITATT1.Model
{
    public class Aluno
    {
        [Key]
        public int id { get; set; } 
        [Required]
        [StringLength(35)]
        public string nome   { get; set; }  
        [Required]
        [StringLength(40)]
        [EmailAddress]
        public string email { get; set; }   
        [Required]
        public int idade {  get; set; }
        [Required]
        public string Endereco { get; set; }
    }
}
