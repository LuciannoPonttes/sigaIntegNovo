using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CargaSigaDoc.Repositorio;

namespace CargaSigaDoc.Negocio
{
    class CargosBO
    {
        private int codDependencia;
        private List<baseCargo> listaCargosBase;
        private GestorhDAO gestorhDao;
        private SigaDAO sigaDao;

        public CargosBO(int p, GestorhDAO gestorhDao, SigaDAO sigaDao)
        {
            // TODO: Complete member initialization
            this.codDependencia = p;
            this.gestorhDao = gestorhDao;
            this.sigaDao = sigaDao;

            if (this.listaCargosBase != null)
            {
              //  this.listaCargosBase = this.gestorhDao.getCargos().ToList();
            }
        }


        public List<baseCargo> Tratar()
        {
            //List<baseCargo> cargosSiga = sigaDao.getCargos(codDependencia);
            //Se houver cargos no Siga que estejam extintos no gestorh, remover do arquivo
            //Cargos específicos

            return this.listaCargosBase;
        }
    }
}
