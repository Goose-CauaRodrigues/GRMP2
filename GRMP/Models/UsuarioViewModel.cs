using System.ComponentModel.DataAnnotations;

namespace GRMP.Models
{
    public class UsuarioViewModel
    {
        //-----------------------------
        // PROPRIEDADES
        //-----------------------------

        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(255)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Digite um email válido")]
        [StringLength(150)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Selecione o nível de acesso")]
        public int NvAcesso { get; set; }
    }
}