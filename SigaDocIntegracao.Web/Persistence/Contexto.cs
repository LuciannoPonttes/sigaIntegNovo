using Microsoft.EntityFrameworkCore;
using SigaDocIntegracao.Web.Models.ModuloEmail;
using SigaDocIntegracao.Web.UsuarioContexto.Models;

namespace SigaDocIntegracao.Web.Persistence
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions<Contexto> options)
            : base(options)
        {
        }

        public DbSet<ExModeloEmailParamModel> ModelExModeloEmailParam { get; set; }
        public DbSet<DocAssinadoEmailHistoricoModel> DocAssinadoEmailHistorico { get; set; }        
        public DbSet<Permissao> Permissao { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<UsuarioPermissao> UsuarioPermissao { get; set; }
    }
}

