using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using SigaDocIntegracao.Web.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SigaDocIntegracao.Web.UsuarioContexto.Models
{
    [Table("tb_usuario")]
    public class Usuario
    {
        public Usuario()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow.AddHours(-3);
            UpdatedAt = null;
            Status = true;
        }

        public static Usuario CriarUsuario(string nome,
            string email,
            Tipo tipo,            
            string matricula)
        {
            var usuario = new Usuario()
            {
                Nome = (!string.IsNullOrEmpty(nome) ? nome.Trim() : string.Empty),
                NomeNormalizado = nome.RemoveCaracteresEspeciais(),
                Email = (!string.IsNullOrEmpty(email) ? email.Trim() : string.Empty),
                Tipo = tipo,
                Matricula = (!string.IsNullOrEmpty(matricula) ? matricula.Trim() : string.Empty),
                StatusCadastro = StatusCadastro.Ativo
            };

            return usuario;
        }

        [Column("Id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [Column("status")]
        public bool Status { get; set; }

        [Column("Nome")]
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(150, ErrorMessage = "Nome pode ter no máximo 150 caracteres.")]
        public string Nome { get; set; }

        [Column("NomeNormalizado")]
        [StringLength(150, ErrorMessage = "NomeNormalizado pode ter no máximo 150 caracteres.")]
        public string NomeNormalizado { get; set; }

        [Column("Email")]
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress]
        [StringLength(256, ErrorMessage = "Email pode ter no máximo 256 caracteres.")]
        public string Email { get; set; }

        [Column("Matricula")]
        [Required(ErrorMessage = "Matricula é obrigatório")]
        [StringLength(100, ErrorMessage = "Matricula pode ter no máximo 100 caracteres.")]
        public string Matricula { get; set; }

        [Column("Tipo")]
        [Required(ErrorMessage = "Tipo é obrigatório")]
        public Tipo Tipo { get; set; }

        [Column("StatusCadastro")]
        [Required(ErrorMessage = "StatusCadastro é obrigatório")]
        public StatusCadastro StatusCadastro { get; set; }

        // EF prop
        public List<Permissao> Permissoes { get; set; }

        public void AdicionarDataAtualizacao()
        {
            UpdatedAt = DateTime.UtcNow.AddHours(-3);
        }

        public void Editar(string nome,            
            string email,
            Tipo tipo,
            StatusCadastro statusCadastro)
        {
            Nome = (!string.IsNullOrEmpty(nome) ? nome.Trim() : string.Empty);
            NomeNormalizado = nome.RemoveCaracteresEspeciais();
            Email = (!string.IsNullOrEmpty(email) ? email.Trim() : string.Empty);
            Tipo = tipo;
            StatusCadastro = statusCadastro;
            AdicionarDataAtualizacao();
        }
    }
}