using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Data;

namespace CargaSigaDoc.Repositorio
{
    public class SQLServerDAO
    {
        // Passar a string de conexão para a instância da classe Database
       // Database gestorhdb; 
        public Dictionary<int, Dictionary<String,DateTime>> dictCriacao = new Dictionary<int, Dictionary<String, DateTime>>();
        //private readonly string connectionStringGestorRh = "Server=10.0.17.111\\CORP2,65225;Database=IFRCORP;User Id=usr_sigadoc_cons;Password=q42jhz2kp3q2";
        private string connectionStringGestorRh => Environment.GetEnvironmentVariable("CONNECTION_STRING_GESTOR_RH");
        public baseCargo[] getCargos()
        {
            List<baseCargo> lista = new List<baseCargo>();

            using (SqlConnection connection = new SqlConnection(connectionStringGestorRh))
            {
                connection.Open();
                string query = "SELECT DISTINCT CAR_CODIGO, CAR_NOME, CAR_SIGLA " +
                               "FROM CARGOS " +
                               "WHERE CAR_DATA_EXTINCAO IS NULL " +
                               "AND CAR_SIGLA NOT IN ('401.ASG', '403.OMT', '405.AJE', '406.AUE', '408.MGE', '323.AXP', '337.TEL')";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new baseCargo
                            {
                                id = (short)reader.GetDecimal(reader.GetOrdinal("CAR_CODIGO")),
                                idSpecified = true,
                                nome = reader.GetString(reader.GetOrdinal("CAR_NOME")),
                                sigla = reader.IsDBNull(reader.GetOrdinal("CAR_SIGLA")) ? "" : reader.GetString(reader.GetOrdinal("CAR_SIGLA"))
                            });
                        }
                    }
                }
            }

            // Adicionando terceiros e estagiários
            lista.Add(new baseCargo { id = 10000, idSpecified = true, nome = "ESTAGIÁRIO", sigla = "ESTAG" });
            lista.Add(new baseCargo { id = 10001, idSpecified = true, nome = "TERCEIRO", sigla = "TERC" });

            return lista.ToArray();
        }


        public baseFuncao[] getFuncoes()
        {
            List<baseFuncao> lista = new List<baseFuncao>();

            using (SqlConnection connection = new SqlConnection(connectionStringGestorRh))
            {
                connection.Open();
                string query = @"SELECT DISTINCT FUN_CODIGO, FUN_DESCRICAO, FUN_SIGLA 
                         FROM CARGOS_CONFIANCA 
                         WHERE FUN_DATA_EXTINCAO IS NULL AND FUN_DESCRICAO IS NOT NULL";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var funcao = new baseFuncao
                            {
                                id = (short)reader.GetDecimal(reader.GetOrdinal("FUN_CODIGO")),
                                idSpecified = true,
                                nome = reader.GetString(reader.GetOrdinal("FUN_DESCRICAO")),
                                sigla = reader.IsDBNull(reader.GetOrdinal("FUN_SIGLA")) ? "" : reader.GetString(reader.GetOrdinal("FUN_SIGLA"))
                            };

                            if (funcao.nome.Equals("PRESIDENTE CEO"))
                            {
                                funcao.nome = "PRESIDENTE";
                            }

                            lista.Add(funcao);
                        }
                    }
                }
            }

            return lista.ToArray();
        }

        public baseLotacao[] getLotacoes(int idDependencia)
        {
            Dictionary<string, DateTime> dictlotas = new Dictionary<string, DateTime>();
            List<baseLotacao> lista = new List<baseLotacao>();

            using (SqlConnection connection = new SqlConnection(connectionStringGestorRh))
            {
                connection.Open();
                string query = @"
            SELECT UOR_CODIGO, UOR_NOME, UOR_SIGLA, UOR_UOR_CODIGO,
                   CASE UOR_TIPO_UNIDAD_ORG 
                       WHEN 1 THEN 'Presidência'
                       WHEN 2 THEN 'Diretoria'
                       WHEN 3 THEN 'Superintendência'
                       WHEN 4 THEN 'Gerência'
                       WHEN 5 THEN 'Coordenação'
                       WHEN 6 THEN 'Encarreg.de atividade'
                       WHEN 7 THEN 'GNA'
                       WHEN 8 THEN 'UTA'
                       WHEN 15 THEN 'Assessoria'
                       WHEN 20 THEN 'EPTA'
                       WHEN 18 THEN 'Setor'
                   END TIPO_UNIDADE,
                   UOR_DATA_CRIACAO
            FROM UNIDADES_ORGANIZACIONAIS 
            WHERE UOR_DATA_EXTINCAO IS NULL 
              AND UOR_TIPO_UNIDAD_ORG != 14  /* CEDIDOS */
              AND UOR_DEP_CODIGO = @idDependencia 
              AND UOR_TIPO_UNIDAD_ORG IS NOT NULL 
              AND UOR_CODIGO NOT IN (16933, 16989, 16996, 42044)
            ORDER BY 
                CASE 
                   WHEN UOR_TIPO_UNIDAD_ORG = 1 THEN 1
                   WHEN UOR_TIPO_UNIDAD_ORG = 2 THEN 2
                   WHEN UOR_TIPO_UNIDAD_ORG = 3 THEN 3
                   WHEN UOR_TIPO_UNIDAD_ORG = 4 THEN 4
                   WHEN UOR_TIPO_UNIDAD_ORG = 20 THEN 4
                   ELSE UOR_TIPO_UNIDAD_ORG
                END, UOR_SIGLA";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idDependencia", idDependencia);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            baseLotacao b = new baseLotacao
                            {
                                id = (long)reader.GetDecimal(reader.GetOrdinal("UOR_CODIGO")),
                                idSpecified = true,
                                nome = reader.GetString(reader.GetOrdinal("UOR_NOME")),
                                sigla = reader.GetString(reader.GetOrdinal("UOR_SIGLA")),
                                tipoLotacao = reader.GetString(reader.GetOrdinal("TIPO_UNIDADE")),
                                idPai = reader["UOR_UOR_CODIGO"] != DBNull.Value
                            ? Convert.ToDecimal(reader["UOR_UOR_CODIGO"]).ToString()
                            : "0"
                            };

                            dictlotas.Add(
                                b.sigla,
                                reader.IsDBNull(reader.GetOrdinal("UOR_DATA_CRIACAO"))
                                    ? new DateTime(2022, 08, 11)
                                    : reader.GetDateTime(reader.GetOrdinal("UOR_DATA_CRIACAO"))
                            );

                            lista.Add(b);
                        }
                    }
                }
            }

            dictCriacao.Add(idDependencia, dictlotas);

            return lista.ToArray();
        }

        public List<Int32> getLotacoesCedidos(int idDependencia)
        {
            String uorsCessao = String.Empty;
            if (idDependencia > 0)
                uorsCessao = String.Format("AND UOR_DEP_CODIGO = {0}", idDependencia);

            List<Int32> lista = new List<Int32>();

            using (SqlConnection connection = new SqlConnection(connectionStringGestorRh))
            {
                connection.Open();
                string query = @"
            SELECT UOR_CODIGO
            FROM UNIDADES_ORGANIZACIONAIS 
            WHERE UOR_DATA_EXTINCAO IS NULL 
              AND UOR_TIPO_UNIDAD_ORG = 14 /* CEDIDOS */
              " + uorsCessao;

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add((Int32)reader.GetDecimal(reader.GetOrdinal("UOR_CODIGO")));
                        }
                    }
                }
            }

            return lista;
        }

        public List<basePessoa> getEmpregados(Dependencia dep, List<Int32> lotCedidos)
        {
            List<basePessoa> empregados = new List<basePessoa>();
            string listaCedidos = String.Empty;

            if (lotCedidos != null && lotCedidos.Count > 0)
                listaCedidos = String.Format("AND EMP_UOR_CODIGO_LOTACAO NOT IN ({0})", String.Join(",", lotCedidos));

            string query = String.Format(@"
        SELECT
            EMP_NUMERO_CPF CPF,
            EMP_NOME NOME,
            EMP_NUMERO_MATRICULA MATRICULA,
            EMP_QLP_CAR_CODIGO CARGO, 
            EMP_STATUS SITUACAO,
            EMP_UOR_CODIGO_LOTACAO LOTACAO,
            EMP_NU_CARTEIRA_IDENTIDADE RG,
            EMP_SIGLA_ORGAO_EMITENTE_CI RGORGAO,
            EMP_ENDERECO_ELETRONICO_MAIL EMAIL,
            EMP_QFU_FUN_CODIGO FUNCAOCONFIANCA,
            'Servidor' TIPO,
            0 ESTCIVIL, 
            EMP_INDICADOR_SEXO SEXO
        FROM EMPREGADOS_INFRAERO
        WHERE EMP_DEP_CODIGO_LOTACAO = @depCodigo 
          AND EMP_STATUS = 1 {0}", listaCedidos);

            using (SqlConnection connection = new SqlConnection(connectionStringGestorRh))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@depCodigo", dep.codigo);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            basePessoa p = new basePessoa();

                            p.matricula = reader.GetInt32(reader.GetOrdinal("MATRICULA"));
                            p.matriculaSpecified = true;

                            p.id = (long)p.matricula;
                            p.idSpecified = true;

                            p.cpf = Decimal.ToInt64(reader.GetDecimal(reader.GetOrdinal("CPF")));
                            p.cpfSpecified = true;

                            p.nome = reader.GetString(reader.GetOrdinal("NOME"));

                            p.cargo = reader.IsDBNull(reader.GetOrdinal("CARGO")) ? 0 : reader.GetInt32(reader.GetOrdinal("CARGO"));
                            p.cargoSpecified = true;

                            p.padraoReferencia = "";

                            p.situacao = (int)reader.GetDecimal(reader.GetOrdinal("SITUACAO"));
                            p.situacaoSpecified = true;

                            p.lotacao = (long)reader.GetInt32(reader.GetOrdinal("LOTACAO"));
                            p.lotacaoSpecified = true;

                            p.rg = reader["RG"] != DBNull.Value
                            ? reader.GetString(reader.GetOrdinal("RG"))
                            : "";
                            p.rgSpecified = true;

                            p.rgOrgao = reader["RGORGAO"] != DBNull.Value
                            ? reader.GetString(reader.GetOrdinal("RGORGAO"))
                            : "";

                            p.email = !reader.IsDBNull(reader.GetOrdinal("EMAIL"))
                                ? reader.GetString(reader.GetOrdinal("EMAIL"))
                                : String.Format("i{0}@infraero.gov.br", p.matricula.ToString());

                            p.funcaoConfianca = reader.IsDBNull(reader.GetOrdinal("FUNCAOCONFIANCA"))
                                ? ""
                                : reader.GetInt32(reader.GetOrdinal("FUNCAOCONFIANCA")).ToString();

                            p.tipo = reader.GetString(reader.GetOrdinal("TIPO"));

                            p.estCivil = reader.GetInt32(reader.GetOrdinal("ESTCIVIL"));
                            p.estCivilSpecified = true;

                            p.sexo = reader.IsDBNull(reader.GetOrdinal("SEXO")) ? "" : reader.GetString(reader.GetOrdinal("SEXO"));

                            p.sigla = "i" + p.matricula.ToString().PadLeft(7, '0');

                            empregados.Add(p);
                        }
                    }
                }
            }

            return empregados;
        }

        public List<basePessoa> getEstagiarios(Dependencia dep)
        {
            List<basePessoa> lista = new List<basePessoa>();

            using (SqlConnection connection = new SqlConnection(connectionStringGestorRh))
            {
                connection.Open();
                string query = @"
            SELECT CEP.NUM_CPF, CEP.NOM_ESTAGIARIO, CEL.UOR_CODIGO, CEL.END_EMAIL_FUNCIONAL, CEP.TIP_SEXO
            FROM CAD_ESTAGIARIO CE
            LEFT JOIN CAD_ESTAGIARIO_PESSOAL CEP ON (CEP.SEQ_ESTAGIARIO_PESSOAL = CE.SEQ_ESTAGIARIO_PESSOAL)
            LEFT JOIN CAD_ESTAGIARIO_LOCALIZACAO CEL ON (CEL.SEQ_ESTAGIARIO = CE.SEQ_ESTAGIARIO)
            LEFT JOIN UNIDADES_ORGANIZACIONAIS UOR ON (UOR.UOR_CODIGO = CEL.UOR_CODIGO)
            WHERE CE.FLG_DESLIGADO = 'N'
            AND UOR.UOR_DATA_EXTINCAO IS NULL
            AND CEL.DAT_TERMINO IS NULL
            AND DEP_CODIGO = @DepCodigo
            ORDER BY NOM_ESTAGIARIO";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DepCodigo", dep.codigo);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            basePessoa p = new basePessoa();
                            string matricula = reader.GetDecimal(reader.GetOrdinal("NUM_CPF")).ToString().PadLeft(11, '0').Substring(0, 9);
                            p.matricula = Int32.Parse(matricula);
                            p.matriculaSpecified = true;

                            try
                            {
                                p.id = (long)p.matricula;
                                p.idSpecified = true;
                                p.cpf = Decimal.ToInt64(reader.GetDecimal(reader.GetOrdinal("NUM_CPF")));
                                p.cpfSpecified = true;
                                p.nome = reader.GetString(reader.GetOrdinal("NOM_ESTAGIARIO"));
                                p.cargo = 10000;
                                p.cargoSpecified = true;
                                p.situacao = 1;
                                p.situacaoSpecified = true;
                                p.lotacao = (long)reader.GetDecimal(reader.GetOrdinal("UOR_CODIGO"));
                                p.lotacaoSpecified = true;
                                p.email = !reader.IsDBNull(reader.GetOrdinal("END_EMAIL_FUNCIONAL"))
                                    ? reader.GetString(reader.GetOrdinal("END_EMAIL_FUNCIONAL"))
                                    : String.Format("i{0}@infraero.gov.br", p.matricula.ToString());
                                p.tipo = "Estagiário";
                                p.sexo = reader.IsDBNull(reader.GetOrdinal("TIP_SEXO")) ? "" : reader.GetString(reader.GetOrdinal("TIP_SEXO"));
                                p.sigla = "t" + p.cpf.ToString();
                            }
                            catch (Exception)
                            {
                                throw;
                            }

                            lista.Add(p);
                        }
                    }
                }
            }

            return lista;
        }

        public List<Int32> getEmpregadosAtivos()
        {
            List<Int32> ativos = new List<Int32>();

            using (SqlConnection connection = new SqlConnection(connectionStringGestorRh))
            {
                connection.Open();
                string query = @"
            SELECT EMP_NUMERO_MATRICULA MATRICULA
            FROM EMPREGADOS_INFRAERO
            WHERE EMP_STATUS = 1 
              AND UOR_SIGLA NOT LIKE 'CEDIDO%' 
              AND UOR_SIGLA NOT IN ('SINA','PEAC') 
              AND DEP_SIGLA NOT IN ('SBSV')";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ativos.Add(reader.GetInt32(reader.GetOrdinal("MATRICULA")));
                        }
                    }
                }
            }

            return ativos;
        }

    }
}
