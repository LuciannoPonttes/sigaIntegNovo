using SigaDocIntegracao.Web.UsuarioContexto.Models;

namespace SigaDocIntegracao.Web.UsuarioContexto.ViewModel
{
    public class ListarUsuarioPaginadoViewModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<Usuario> Usuarios { get; set; }
    }
}
