using System.ComponentModel.DataAnnotations;

namespace GRMP.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Senha {  get; set; }
        [Required]
        public string Email {  get; set; }
    }
}
