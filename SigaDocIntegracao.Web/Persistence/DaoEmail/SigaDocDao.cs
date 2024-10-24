using Oracle.ManagedDataAccess.Client;
using SigaDocIntegracao.Web.Models.ModuloEmail;

namespace SigaDocIntegracao.Web.Persistence.DaoEmail
{
    public class SigaDocDao
    {
        private string connectionStringSigaDoc => Environment.GetEnvironmentVariable("CONNECTION_STRING_SIGADOC_BASE_SIGA");
        public List<ExDocAssinadoModel> getDocumentosAssinados(ExModeloEmailParamModel modelo)
        {
            List<ExDocAssinadoModel> documentosAssinados = new List<ExDocAssinadoModel>();

            using (OracleConnection db = new OracleConnection(connectionStringSigaDoc))
            {
                db.Open();

                // Query com parâmetros
                string query = $@"
            select distinct
                          o.acronimo_orgao_usu || '-' || f.sigla_forma_doc || '-' || doc.ano_emissao || '/' || 
                          case 
                              when doc.num_expediente > 99999 then to_char(doc.num_expediente)
                              else lpad(doc.num_expediente, 5, 0)
                          end as nome,
                          m.nm_mod
                    from (
                          select
                              sum(
                                  case 
                                      when mov.id_tp_mov in (1, 24) then 1
                                      else 0
                                  end
                              ) as assinantes,
                              sum(
                                  case 
                                      when mov.id_tp_mov in (11, 58, 22, 59, 72, 25, 26, 65, 45, 60) then 1
                                      else 0
                                  end
                              ) as assinaturas,
                              mobil.id_doc
                          from ex_movimentacao mov
                          inner join ex_mobil mobil on mov.id_mobil = mobil.id_mobil
                          inner join ex_tipo_movimentacao tp on mov.id_tp_mov = tp.id_tp_mov
                          where mov.id_mov_canceladora is null 
                          group by mobil.id_doc
                     ) a
                     inner join ex_documento doc on a.id_doc = doc.id_doc
                     inner join ex_forma_documento f on doc.id_forma_doc = f.id_forma_doc
                     inner join corporativo.cp_orgao_usuario o on doc.id_orgao_usu = o.id_orgao_usu
                     inner join ex_modelo m on doc.id_mod = m.id_mod
                     inner join ex_mobil mob on doc.id_doc = mob.id_doc
                     inner join ex_movimentacao mov
                         on mov.id_mobil = mob.id_mobil
                         and mov.dt_ini_mov >= to_date(:data, 'dd/mm/yyyy hh24:mi') -- data/hora de última execução
                     where 
                         a.assinantes = a.assinaturas and a.assinantes != 0
                          and doc.id_mod in (
                              select id_mod 
                              from ex_modeloteste
                              where his_id_ini in (:idsSigaDoc)
         )";

                using (OracleCommand cmd = new OracleCommand(query, db))
                {
                    // Adiciona os parâmetros
                    cmd.Parameters.Add(new OracleParameter("data", modelo.DataInicio.ToString("dd/MM/yyyy HH:mm")));
                    cmd.Parameters.Add(new OracleParameter("idsSigaDoc", string.Join(",", modelo.IdSigaDoc)));

                    using (OracleDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            // Cria uma nova instância de ExDocAssinadoModel e popula os campos
                            ExDocAssinadoModel docAssinado = new ExDocAssinadoModel
                            {
                                CodigoDocumento = r.GetString(r.GetOrdinal("NOME")),
                                ModeloDocumento = r.GetString(r.GetOrdinal("NM_MOD"))
                            };

                            // Adiciona o objeto à lista de resultados
                            documentosAssinados.Add(docAssinado);
                        }
                    }
                }
            }

            return documentosAssinados;
        }

        public List<ExModeloModel> getModelos()
        {
            List<ExModeloModel> modelos = new List<ExModeloModel>();

            using (OracleConnection db = new OracleConnection(connectionStringSigaDoc))
            {
                db.Open();

                string query = @"SELECT HIS_ID_INI, NM_MOD 
                                FROM SIGA.ex_modelo 
                                WHERE his_ativo = 1 
                                AND HIS_ID_INI IS NOT NULL
                                AND NM_MOD IS NOT NULL
                                            ";

                using (OracleCommand cmd = new OracleCommand(query, db))
                {
                    using (OracleDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            // Cria uma nova instância de ExModeloModel e popula os campos
                            ExModeloModel modelo = new ExModeloModel
                            {
                                CodigoSigaDoc = r.GetString(r.GetOrdinal("HIS_ID_INI")),
                                NomeModelo = r.GetString(r.GetOrdinal("NM_MOD"))
                            };

                            // Adiciona o modelo à lista de resultados
                            modelos.Add(modelo);
                        }
                    }
                }
            }

            return modelos;
        }
    }
}
