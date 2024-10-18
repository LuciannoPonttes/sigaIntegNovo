using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SigaDocIntegracao.Web.Models.ModuloEmail;
using SigaDocIntegracao.Web.Persistence;
using SigaDocIntegracao.Web.Persistence.DaoEmail;
using System.Collections.Generic;

namespace SigaDocIntegracao.Web.Service
{
    public class EmailService
    {
        private readonly Contexto _contexto;

        public EmailService(Contexto contexto)
        {
            _contexto = contexto;
        }

        public List<ExDocAssinadoModel> GetDocumentosAssinados(ExModeloEmailParamModel modelo) {
            SigaDocDao sigaDao = new SigaDocDao();
            return sigaDao.getDocumentosAssinados(modelo);
        
        }

        // GET: ExModeloEmailParam
        public async Task<List<ExModeloEmailParamModel>> GetModelosCadastradosAsync()
        {
            return await _contexto.ModelExModeloEmailParam.ToListAsync();
        }

        public async Task<List<DocAssinadoEmailHistoricoModel>> GetStatusDocumentosAsync()
        {
            return await _contexto.DocAssinadoEmailHistorico.ToListAsync();
        }

        public void salvarDocAssinadoEmailHistoricoModel(DocAssinadoEmailHistoricoModel DocAssinadoEmailHistoricoModel)
        {
            _contexto.Add(DocAssinadoEmailHistoricoModel);
             _contexto.SaveChangesAsync();
        }
        

        public async void alterarDocAssinadoEmailHistoricoModel(DocAssinadoEmailHistoricoModel DocAssinadoEmailHistoricoModel)
        {
            _contexto.Update(DocAssinadoEmailHistoricoModel); ;
            await _contexto.SaveChangesAsync();
        }

        public List<ExModeloModel> GetModelos()
        {
            SigaDocDao sigaDao = new SigaDocDao();
            return sigaDao.getModelos();

        }

    }
}
