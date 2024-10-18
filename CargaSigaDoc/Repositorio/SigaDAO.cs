using Oracle.ManagedDataAccess.Client;
using System.Text;
using System.Data;

namespace CargaSigaDoc.Repositorio
{
    public class SigaDAO
    {
        // Database db;
        //private string connectionStringSigaDoc = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=10.0.26.79)(PORT=1521))(CONNECT_DATA=(SID=ifrsiga)));User Id=corporativo;Password=qsxcasf12309";
        private string connectionStringSigaDoc => Environment.GetEnvironmentVariable("CONNECTION_STRING_SIGADOC");
        /*
         // Método Siga Infraero
        public List<Dependencia> getDependencias()
        {
            //pegar dados das dependências
            List<Dependencia> dependencias = db.ExecuteQuery(
                                    @"select COD_ORGAO_USU, SIGLA_ORGAO_USU, DEP_DATA_CONCESSAO
                                      from dependencias
                                      inner join CORPORATIVO.CP_ORGAO_USUARIO on (DEP_CODIGO = COD_ORGAO_USU)
                                      where DEP_DATA_EXTINCAO IS NULL
                                      order by DEP_DATA_CONCESSAO",
                                    r =>
                                    {
                                        return new Dependencia
                                        {
                                            codigo = r.GetInt32("COD_ORGAO_USU"),
                                            siglaSiga = r.GetString("SIGLA_ORGAO_USU"),
                                            isExtinta = r.IsDBNull("DEP_DATA_CONCESSAO") ? false : true
                                        };
                                    }, null);
            return dependencias;
        }
        */
        //Método Siga NAV
        public List<Dependencia> getDependencias()
        {
            List<Dependencia> dependencias = new List<Dependencia>();

            using (OracleConnection db = new OracleConnection(connectionStringSigaDoc))
            {
                db.Open();

                using (OracleCommand cmd = new OracleCommand(
                    @"select COD_ORGAO_USU, SIGLA_ORGAO_USU, 'ATIVA' DEP_DATA_CONCESSAO
                  from CORPORATIVO.CP_ORGAO_USUARIO
                  where ID_ORGAO_USU != 9999999999 AND COD_ORGAO_USU is not null", db))
                {
                    using (OracleDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            Dependencia dependencia = new Dependencia
                            {
                                codigo = r.GetInt32(r.GetOrdinal("COD_ORGAO_USU")),
                                siglaSiga = r.GetString(r.GetOrdinal("SIGLA_ORGAO_USU")),
                                isExtinta = r.IsDBNull(r.GetOrdinal("DEP_DATA_CONCESSAO")) ? false : true
                            };

                            dependencias.Add(dependencia);
                        }
                    }
                }
            }

            return dependencias;
        }
        public List<baseCargo> GetCargos(int codDependencia)
        {
            List<baseCargo> cargos = new List<baseCargo>();

            
            using (OracleConnection db = new OracleConnection(connectionStringSigaDoc))
            {
                db.Open();

                string query = "select IDE_CARGO, NOME_CARGO from corporativo.DP_CARGO where ID_ORGAO_USU = :codDependencia";

                using (OracleCommand cmd = new OracleCommand(query, db))
                {
                    // Adiciona o parâmetro com o valor fornecido
                    cmd.Parameters.Add(new OracleParameter("codDependencia", codDependencia));

                    using (OracleDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            baseCargo cargo = new baseCargo
                            {
                                id = (short)r.GetInt32(r.GetOrdinal("IDE_CARGO")),
                                idSpecified = true,
                                nome = r.GetString(r.GetOrdinal("NOME_CARGO"))
                            };

                            cargos.Add(cargo);
                        }
                    }
                }
            }

            return cargos;
        }

        public List<baseLotacao> GetLotacoes(int codDependencia)
        {
            List<baseLotacao> lotacoes = new List<baseLotacao>();

          
            using (OracleConnection db = new OracleConnection(connectionStringSigaDoc))
            {
                db.Open();

                string query = "select ID_LOTACAO, IDE_LOTACAO, NOME_LOTACAO, SIGLA_LOTACAO, ID_LOTACAO_PAI from CORPORATIVO.DP_LOTACAO where DATA_FIM_LOT is null AND ID_ORGAO_USU = :codDependencia";

                using (OracleCommand cmd = new OracleCommand(query, db))
                {
                    // Adiciona o parâmetro com o valor fornecido
                    cmd.Parameters.Add(new OracleParameter("codDependencia", codDependencia));

                    using (OracleDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            baseLotacao lotacao = new baseLotacao
                            {
                                Value = r.GetInt32(r.GetOrdinal("ID_LOTACAO")).ToString(),
                                id = Int64.Parse(r.GetString(r.GetOrdinal("IDE_LOTACAO"))),
                                idSpecified = true,
                                nome = r.GetString(r.GetOrdinal("NOME_LOTACAO")),
                                sigla = r.GetString(r.GetOrdinal("SIGLA_LOTACAO")),
                                idPai = r.IsDBNull(r.GetOrdinal("ID_LOTACAO_PAI")) ? "0" : r.GetInt32(r.GetOrdinal("ID_LOTACAO_PAI")).ToString(),
                            };

                            lotacoes.Add(lotacao);
                        }
                    }
                }
            }

            return lotacoes;
        }

        public basePessoa[] GetTerceiros(Dependencia dep)
        {
            List<basePessoa> lista = new List<basePessoa>();

            
            using (OracleConnection db = new OracleConnection(connectionStringSigaDoc))
            {
                db.Open();

                string query = @"select distinct
                            DP_PESSOA.MATRICULA,
                            DP_PESSOA.CPF_PESSOA CPF,
                            DP_PESSOA.NOME_PESSOA NOME,
                            LOT.IDE_LOTACAO LOTACAO,
                            DP_PESSOA.ID_CARGO CARGO,
                            DP_PESSOA.EMAIL_PESSOA EMAIL,
                            DP_PESSOA.SEXO_PESSOA SEXO,
                            TO_CHAR(DP_PESSOA.DATA_INICIO_EXERCICIO_PESSOA,'DD/MM/YYYY') DTINICIOEXERCICIO,
                            DP_PESSOA.SITUACAO_FUNCIONAL_PESSOA SITUACAO,
                            DP_PESSOA.ID_TP_PESSOA TIPO
                        from CORPORATIVO.DP_PESSOA 
                        left join DP_LOTACAO LOT ON (LOT.ID_LOTACAO = DP_PESSOA.ID_LOTACAO)
                        where ID_TP_PESSOA IN (4) AND DP_PESSOA.ID_ORGAO_USU = :codDependencia and DATA_FIM_PESSOA IS NULL";

                using (OracleCommand cmd = new OracleCommand(query, db))
                {
                    // Adiciona o parâmetro com o valor fornecido
                    cmd.Parameters.Add(new OracleParameter("codDependencia", dep.codigo));

                    using (OracleDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            basePessoa p = new basePessoa();

                            try
                            {
                                p.matricula = r.GetInt32("MATRICULA");
                                p.matriculaSpecified = true;

                                p.id = (long)r.GetInt32("MATRICULA");
                                p.idSpecified = true;

                                p.cpf = r.GetInt64("CPF");
                                p.cpfSpecified = true;

                                p.nome = r.GetString("NOME");

                                if (!r.IsDBNull(r.GetOrdinal("DTINICIOEXERCICIO")))
                                {
                                    p.dtInicioExercicio = r.GetString("DTINICIOEXERCICIO");
                                    p.dtInicioExercicioSpecified = true;
                                }
                                else
                                {
                                    p.dtInicioExercicioSpecified = false;
                                }

                                p.situacao = Int32.Parse(r.GetString("SITUACAO"));
                                p.situacaoSpecified = true;

                                p.lotacao = Int64.Parse(r.GetString("LOTACAO"));
                                p.lotacaoSpecified = true;

                                p.email = !r.IsDBNull(r.GetOrdinal("EMAIL"))
                                    ? r.GetString("EMAIL")
                                    : $"t{r.GetString("CPF").PadLeft(11, '0').Substring(0,9)}@infraero.gov.br";

                                int tipo = r.GetInt32("TIPO");

                                if (tipo == 3)
                                {
                                    p.tipo = "Estagiário";
                                    p.cargo = 10000;
                                    p.cargoSpecified = true;
                                }
                                else if (tipo == 4)
                                {
                                    p.tipo = "Terceirizado";
                                    p.cargo = 10001;
                                    p.cargoSpecified = true;
                                }

                                p.sexo = r.IsDBNull(r.GetOrdinal("SEXO")) ? "" : r.GetString("SEXO");

                                p.sigla = $"t{p.cpf:D9}";
                            }
                            catch (FormatException)
                            {
                                // Se um erro de formato ocorrer, apenas continue para o próximo registro
                                continue;
                            }
                            catch (Exception ex)
                            {
                                // Propaga outros erros
                                throw;
                            }

                            lista.Add(p);
                        }
                    }
                }
            }

            return lista.ToArray();
        }

        public string GetNome(int matricula)
        {
            string nome = null;

            
            using (OracleConnection db = new OracleConnection(connectionStringSigaDoc))
            {
                db.Open();

                string query = "select NOME_PESSOA from DP_PESSOA where Matricula = :matricula AND DATA_FIM_PESSOA IS NULL";

                using (OracleCommand cmd = new OracleCommand(query, db))
                {
                    // Adiciona o parâmetro com o valor fornecido
                    cmd.Parameters.Add(new OracleParameter("matricula", matricula));

                    // Executa o comando e obtém o resultado
                    object result = cmd.ExecuteScalar();

                    // Verifica se o resultado é nulo e, se não for, converte para string
                    if (result != null)
                    {
                        nome = result.ToString();
                    }
                }
            }

            return nome;
        }

        public string GetSiglaFuncao(int idFuncao)
        {
            string siglaFuncao = null;

           
            using (OracleConnection db = new OracleConnection(connectionStringSigaDoc))
            {
                db.Open();

                string query = "select distinct SIGLA_FUNCAO_CONFIANCA from CORPORATIVO.DP_FUNCAO_CONFIANCA where IDE_FUNCAO_CONFIANCA = :idFuncao AND DT_FIM_FUNCAO_CONFIANCA is null";

                using (OracleCommand cmd = new OracleCommand(query, db))
                {
                    // Adiciona o parâmetro com o valor fornecido
                    cmd.Parameters.Add(new OracleParameter("idFuncao", idFuncao));

                    // Executa o comando e obtém o resultado
                    object result = cmd.ExecuteScalar();

                    // Verifica se o resultado é nulo e, se não for, converte para string
                    if (result != null)
                    {
                        siglaFuncao = result.ToString();
                    }
                }
            }

            return siglaFuncao;
        }
    }
}
