using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SigaDocIntegracao.Web.Models.ModuloEmail
{
    [Table("CAD_MODELO_NOTIF_EMAIL")]
    public class ExModeloEmailParamModel
    {
        [Column("ID")]
        [Display(Name ="Codigo")]
        [Key]
        public long Id { get; set; }

        [Column("ID_SIGA_DOC")]
        [Display(Name = "Codigo SIGADOC")]
        public string IdSigaDoc { get; set; }

        [Column("TXT_NOME_NOT")]
        [Display(Name = "Nome da Notificação")]
        [Required(ErrorMessage = "Campo Notificação Obrigatório ")]
        public string NomeNot { get; set; }

        [Column("TXT_DESCRICAO_MODELO")]
        [Display(Name = "Modelo")]
        [Required(ErrorMessage = "Campo Modelo Obrigatório ")]
        public string DescricaoModelo { get; set; }

        [Column("TXT_DESTINATARIOS_EMAILS")]
        [Display(Name = "Destinatários")]
        [Required(ErrorMessage = "Campo Destinatários Obrigatório ")]
        public string Destinatarios { get; set; }

        [Column("TXT_CONTEUDO_EMAIL")]
        [Display(Name = "Conteúdo")]
        [Required(ErrorMessage = "Campo Conteúdo Obrigatório ")]
        public string ConteudoEmail { get; set; }

        [Column("TXT_ASSUNTO")]
        [Display(Name = "Assunto")]
        [Required(ErrorMessage = "Campo Assunto Obrigatório ")]
        public string Assunto { get; set; }

        [Column("FLG_STATUS_ATIVO")]
        [Display(Name = "Ativo")]
        public Boolean Ativo { get; set; }

        [Column("DTH_INICIO_ENVIO")]
        [Display(Name = "Data de Início")]
        [Required(ErrorMessage = "Campo Obrigatório ")]
        public DateTime DataInicio { get; set; }

        [Column("DTH_CRIACAO_NOT")]
        [Display(Name = "Data de Início")]
        public DateTime DataCriacao { get; set; }

        [Column("DTH_ULTIMO_PROCESSAMENTO")]
        [Display(Name = "ULTIMO PROCESSAMENTO")]
        public DateTime DataUltimoProcessamento { get; set; }

    }
}
