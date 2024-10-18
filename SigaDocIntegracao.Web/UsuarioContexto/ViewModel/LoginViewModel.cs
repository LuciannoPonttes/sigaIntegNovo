using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SigaDocIntegracao.Web.UsuarioContexto.ViewModel
{
    public class LoginViewModel
    {
        [StringLength(100, ErrorMessage = "Matricula pode ter no máximo 100 caracteres.")]
        [DisplayName("Matrícula")]
        [Required(ErrorMessage = "Informe a matricula")]
        public string Matricula { get; set; }

        [Required(ErrorMessage = "Informe a senha")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; }
    }
}
