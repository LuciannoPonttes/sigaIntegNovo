using SigaDocIntegracao.Web.UsuarioContexto.Models;
using System.ComponentModel.DataAnnotations;

namespace SigaDocIntegracao.Web.UsuarioContexto.ViewModel
{
    public class UsuarioViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [Length(1, 150, ErrorMessage = "Nome deve ter entre 1 e 150 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Email é obrigatório")]
        [Length(1, 150, ErrorMessage = "Email deve ter entre 1 e 256 caracteres")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Matricula é obrigatório")]
        [StringLength(100, ErrorMessage = "Matricula pode ter no máximo 100 caracteres.")]
        public string Matricula { get; set; }

        [Required(ErrorMessage = "Tipo é obrigatório")]
        public Tipo Tipo { get; set; }

        public StatusCadastro? StatusCadastro { get; set; }

        public List<Permissao>? PermissoesDisponiveis { get; set; }
        public List<Guid>? PermissoesSelecionadas { get; set; }

        public DateTime? CreatedAt { get; set; }        
    }
}
