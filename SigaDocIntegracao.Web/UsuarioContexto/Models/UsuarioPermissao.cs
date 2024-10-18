using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SigaDocIntegracao.Web.UsuarioContexto.Models
{
    [Table("tb_usuario_permissao")]
    public class UsuarioPermissao
    {
        public UsuarioPermissao()
        {
        }

        public UsuarioPermissao(Guid usuarioId, Guid permissaoId)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow.AddHours(-3);
            UpdatedAt = null;
            Status = true;
            UsuarioId = usuarioId;
            PermissaoId = permissaoId;
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

        [Column("usuario_id")]
        [Required(ErrorMessage = "UsuarioId é obrigatório")]
        public Guid UsuarioId { get; set; }

        [Column("permissao_id")]
        [Required(ErrorMessage = "PermissaoId é obrigatório")]
        public Guid PermissaoId { get; set; }

        // EF prop
        public Usuario Usuario { get; set; }
        public Permissao Permissao { get; set; }
    }
}
