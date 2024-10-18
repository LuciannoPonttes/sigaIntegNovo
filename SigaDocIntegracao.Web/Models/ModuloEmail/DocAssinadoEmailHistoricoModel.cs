using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SigaDocIntegracao.Web.Models.ModuloEmail
{
    [Table("CAD_MODELO_EMAIL_HIST")]
    public class DocAssinadoEmailHistoricoModel
    {
        
        [Column("ID")]
        [Key]
        public long Id { get; set; }

        [Column("TXT_DOCUMENTO_NOME")]
        public string Documento { get; set; }

        [Column("FLG_STATUS_ENVIADO")]
        public Boolean Enviado { get; set; }
    }
}
