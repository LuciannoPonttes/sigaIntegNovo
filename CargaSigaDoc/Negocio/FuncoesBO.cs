using CargaSigaDoc.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CargaSigaDoc.Negocio
{
    class FuncoesBO
    {
        private int codDependencia;
        private List<baseFuncao> listaFuncoesBase;
        private GestorhDAO gestorhDao;
        private SigaDAO sigaDao;

        public FuncoesBO(int dep, GestorhDAO gestorhDao, SigaDAO sigaDao)
        {
            // TODO: Complete member initialization
            this.codDependencia = dep;
            this.gestorhDao = gestorhDao;
            this.sigaDao = sigaDao;

            if (this.listaFuncoesBase != null)
            {
             //   this.listaFuncoesBase = this.gestorhDao.getFuncoes().ToList();
            }
        }

        public void Tratar()
        {
            //List<baseFuncao> cargosSiga = sigaDao.getCargos(codDependencia);
            //Se houver cargos no Siga que estejam extintos no gestorh, remover do arquivo
            //Cargos específicos
        }
    }
}
