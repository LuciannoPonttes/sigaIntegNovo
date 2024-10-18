using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SigaDocIntegracao.Web.UsuarioContexto.Models
{
    [Table("tb_permissao")]
    public class Permissao
    {
        public Permissao()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow.AddHours(-3);
            UpdatedAt = null;
            Status = true;
        }

        public Permissao(string nome)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow.AddHours(-3);
            UpdatedAt = null;
            Status = true;
            Nome = (!string.IsNullOrEmpty(nome) ? nome.Trim() : string.Empty);
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

        [Column("nome")]
        [StringLength(100, ErrorMessage = "Nome pode ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        // EF prop
        public List<Usuario> Usuarios { get; set; }

        public void Editar(string nome)
        {
            Nome = (!string.IsNullOrEmpty(nome) ? nome.Trim() : string.Empty);
        }
    }
}