using SigaDocIntegracao.Web.Models.ModuloEmail;

namespace SigaDocIntegracao.Web.ViewModels
{
    public class ListarExModeloEmailParamPaginadoViewModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<ExModeloEmailParamModel> ExModeloEmailParamModels { get; set; }
    }
}
