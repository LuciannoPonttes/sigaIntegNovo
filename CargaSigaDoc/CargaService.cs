using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CargaSigaDoc.Negocio;
using CargaSigaDoc.Repositorio;
using System.Diagnostics;
using Oracle.ManagedDataAccess.Client;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CargaSigaDoc
{
    public class CargaService
    {
        public async void ExecutaXml()
        {
            DateTime dataCorteAtualizacaoEstr = new DateTime(2022, 05, 10);
            //inicialização
            SQLServerDAO daoGestorh = new SQLServerDAO();

            baseCargo[] cargos = daoGestorh.getCargos();
            baseFuncao[] funcoes = daoGestorh.getFuncoes();




            SigaDAO daoSiga = new SigaDAO();
            daoSiga.getDependencias();

            @base xml;
            List<basePessoa> empregadosInseridos = new List<basePessoa>();

            Console.Write("Carregando cargos... ");
            //  baseCargo[] cargos = daoGestorh.getCargos();
            Console.WriteLine("OK!");

            Console.Write("Carregando Funções... ");
            // baseFuncao[] funcoes = daoGestorh.getFuncoes();
            funcoes.ToList().ForEach(x =>
                {
                    if (String.IsNullOrEmpty(x.sigla))
                        x.sigla = daoSiga.GetSiglaFuncao(x.id);
                });
            Console.WriteLine("OK!");

           // String path = String.Concat(Directory.GetCurrentDirectory(), "\\xml\\");
            // StreamWriter script = File.CreateText(String.Concat(path, "execXML.bat"));

            //DirectoryInfo di = new DirectoryInfo(path);
            //foreach (FileInfo file in di.GetFiles())
            //{
                // if(!file.Name.Equals("execXML.bat"))
                //    file.Delete();
            //}

            foreach (var dep in daoSiga.getDependencias())
            {
                //int temp ;
                //if (dep.codigo == 169)
                //    temp = 1;

                xml = new @base();
                Console.Write(String.Format("Carregando informações da dependencia {0}/{1}... ", dep.codigo, dep.siglaSiga));
                DateTime dataHoraGeracao = DateTime.Now;
                xml.orgaoUsuario = dep.siglaSiga;
                xml.dataHora = dataHoraGeracao.ToString("dd/MM/yyyy HH:mm:ss");
                xml.versao = 2;
                xml.versaoSpecified = true;
                xml.aditionalEmails = "diogorocha@infranet.gov.br";
                Console.WriteLine("OK!");

                /*
                Console.Write("Carregando cargos... ");
                CargosBO cargos = new CargosBO(dep.codigo,daoGestorh,daoSiga);
                cargos.Tratar();

                Console.Write("Carregando Funções... ");
                */

                Console.Write("Preechendo XML com cargos e funções... ");
                xml.cargos = cargos;
                xml.funcoes = funcoes;
                Console.WriteLine("OK!");

                Console.Write("Carregando e preenchendo XML com lotações e empregados para a dependência... ");
                //xml.lotacoes = daoGestorh.getLotacoes(dep.codigo);
                var lotacoesSiga = daoSiga.GetLotacoes(dep.codigo);
                var lotacoesGestor = daoGestorh.getLotacoes(dep.codigo).ToList();//.OrderBy(x => x.idPai);

                //foreach (var i in lotacoesSiga.Where(x => x.sigla.Contains("MNPM"))) {
                //    Console.WriteLine("{0} - {1}", i.id, i.sigla);
                //}

                //xml.lotacoes = lotacoesGestor.ToArray();

                List<baseLotacao> lotacoes = new List<baseLotacao>();
                foreach (var lotaGestor in lotacoesGestor)
                {
                    baseLotacao clone = new baseLotacao();
                    {
                        clone.id = lotaGestor.id;
                        clone.idPai = lotaGestor.idPai;
                        clone.idSpecified = lotaGestor.idSpecified;
                        clone.nome = lotaGestor.nome;
                        clone.sigla = lotaGestor.sigla;
                        clone.tipoLotacao = lotaGestor.tipoLotacao;
                        clone.Value = lotaGestor.Value;
                    };

                    // Se a data de criação da lotação for maior que 10/05/2022, carregar do IFRCORP
                    if (daoGestorh.dictCriacao[dep.codigo][lotaGestor.sigla] >= dataCorteAtualizacaoEstr
                        || clone.id.ToString() == clone.idPai)
                    {
                        if (clone.id.ToString() != clone.idPai)
                        {
                            if (!lotacoes.Any(x => x.id.ToString() == clone.idPai))
                                clone.idPai = null;
                        }

                        lotacoes.Add(clone);
                        continue;
                    }

                    //if (clone.id.ToString() == clone.idPai)
                    //    lotacoes.Add(clone);

                    //Nova tentativa - recuperar idPai
                    var lotaSiga = lotacoesSiga.Where(lotSiga => lotSiga.sigla == lotaGestor.sigla); //encontra a lotação no SIGA
                    if (lotaSiga.Count() == 0)
                    {
                        lotacoes.Add(clone); // se não tiver lotação no SIGA (nova), retorna a do IFRCORP
                        continue;
                    }

                    var lotaPaiIFRCORP = lotacoes.Where(lotPaiIFRCORP => lotPaiIFRCORP.id.ToString() == lotaGestor.idPai).FirstOrDefault();
                    if (lotaPaiIFRCORP == null)
                        lotaPaiIFRCORP = lotacoesGestor.Where(lotPaiIFRCORP => lotPaiIFRCORP.id.ToString() == lotaGestor.idPai).FirstOrDefault();
                    //var lotaPaiSiga = lotacoesSiga.Where(lotPaiSiga => lotPaiSiga.id.ToString() == lotaSiga.First().idPai).First();//encontra a lotação pai no SIGA

                    if (lotaSiga != null && lotaSiga.Count() == 1)
                    {
                        clone.id = lotaSiga.First().id;
                    }

                    if (lotaPaiIFRCORP != null)
                    {
                        var bla = lotacoes.Where(x => x.sigla == lotaPaiIFRCORP.sigla).FirstOrDefault();
                        clone.idPai = (bla != null) ? bla.id.ToString() : "0";
                    }
                    else
                        //clone.idPai = (lotaPaiIFRCORP != null) ? lotaPaiIFRCORP.id.ToString() : "0";
                        clone.idPai = "0";

                    if (!lotacoes.Contains(clone))
                        lotacoes.Add(clone);
                }

                xml.lotacoes = lotacoes.ToArray();


#if false
                xml.lotacoes = lotacoesGestor.Select(lotaGestor =>
                    {
                        baseLotacao clone = new baseLotacao();
                        {
                            clone.id = lotaGestor.id;
                            clone.idPai = lotaGestor.idPai;
                            clone.idSpecified = lotaGestor.idSpecified;
                            clone.nome = lotaGestor.nome;
                            clone.sigla = lotaGestor.sigla;
                            clone.tipoLotacao = lotaGestor.tipoLotacao;
                            clone.Value = lotaGestor.Value;
                        };

                        // Se a data de criação da lotação for maior que 10/05/2022, carregar do IFRCORP
                        if (daoGestorh.dictCriacao[dep.codigo][lotaGestor.sigla] >= dataCorteAtualizacaoEstr)
                            return clone;

                        //Original - recuperar idPai
                        /*
                        var resultSiga = lotacoesSiga.Where(lotaSiga => lotaSiga.sigla == lotaGestor.sigla);
                        if (resultSiga != null && resultSiga.Count() > 1)
                            throw new Exception(String.Format("Não pode ter sigla repetida: {0}", lotaGestor.sigla));

                        if (resultSiga != null && resultSiga.Count() == 1)
                        {
                            var lot = resultSiga.First();
                            clone.id = lot.id;
                            var lotPai = lotacoesSiga.Where(z => z.id.ToString() == clone.idPai.ToString()).FirstOrDefault();
                            clone.idPai = (lotPai != null) ? lotPai.id.ToString() : "0";
                        }
                        */

                        //Nova tentativa - recuperar idPai
                        var lotaSiga = lotacoesSiga.Where(lotSiga => lotSiga.sigla == lotaGestor.sigla); //encontra a lotação no SIGA
                        if (lotaSiga.Count() == 0) return clone; // se não tiver lotação no SIGA (nova), retorna a do IFRCORP
                        var lotaPaiIFRCORP = lotacoesGestor.Where(lotPaiIFRCORP => lotPaiIFRCORP.id.ToString() == lotaGestor.idPai).FirstOrDefault();
                        //var lotaPaiSiga = lotacoesSiga.Where(lotPaiSiga => lotPaiSiga.id.ToString() == lotaSiga.First().idPai).First();//encontra a lotação pai no SIGA

                        clone.id = lotaSiga.First().id;
                        clone.idPai = (lotaPaiIFRCORP != null) ? lotaPaiIFRCORP.id.ToString() : "0";



                        //string siglaPai = lotacoesGestor.Where(uorPai => uorPai.id.ToString() == lotaGestor.idPai).FirstOrDefault().sigla;


                        return clone;
                    }).ToArray();
#endif

                //var lotacoesSiglaIgual = lotacoesSiga.Where(lotSiga => lotacoesGestor.Any(lotGestor => lotGestor.sigla == lotSiga.sigla));
                //var lotacoesSiglaDiferente = lotacoesGestor.Where(lotGestor => !lotacoesSiglaIgual.Any(lotSiga => lotSiga.sigla == lotGestor.sigla));

                IEnumerable<long> query1 = xml.lotacoes.Select(x => x.id);
                List<Int32> lotCedidos = daoGestorh.getLotacoesCedidos(dep.codigo);
                lotCedidos.Add(16027);

                // Converte array em lista e...
                var pess = daoGestorh.getEmpregados(dep, lotCedidos).ToList();
                //var presidenteOuDiretor = pess.Where(x => x.matricula == 1876988 || x.matricula == 1883110 || x.matricula == 3283030).FirstOrDefault();
                //if (presidenteOuDiretor != null) {
                //    pess.Remove(presidenteOuDiretor);
                //    presidenteOuDiretor.nome = daoSiga.getNome(presidenteOuDiretor.matricula);
                //    pess.Add(presidenteOuDiretor);
                //}
                pess.ForEach(x =>
                {
                    if (x.matricula == 1886788)
                    {
                        x.nome = daoSiga.GetNome(x.matricula);
                    }
                    /*
                    try
                    {
                        var lotGestor = lotacoesGestor.First(y => y.id == x.lotacao);
                        var lotSiga = lotacoesSiga.First(y => y.sigla == lotGestor.sigla);
                        x.lotacao = lotSiga.id;
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine("Erro ao atribuir código UOR SIGA para a matrícula "+x.matricula);
                    }
                    */

                    if (!lotacoes.Exists(lot => lot.id == x.lotacao))
                    {
                        var lotaPess = lotacoesGestor.Where(lot => lot.id == x.lotacao).FirstOrDefault();
                        if (lotaPess != null)
                        {
                            var lotax = lotacoes.Where(lot => lot.sigla == lotaPess.sigla).FirstOrDefault();
                            if (lotax != null)
                                x.lotacao = lotax.id;
                        }

                    }
                });

                // remove os empregados que não estão na lista de lotações da dependência e ...
                pess.RemoveAll(x =>
                {
                    return !query1.Contains(x.lotacao);
                });

                // remove os empregados que já foram adicionados em dependêcias anteriores
                pess.RemoveAll(x => empregadosInseridos.Select(y => y.matricula).Contains(x.matricula));

                //adiciona estagiários e terceiros
                pess.AddRange(daoGestorh.getEstagiarios(dep).ToList());

                //* Terceiros
                var tercEstList = daoSiga.GetTerceiros(dep).ToList();
                var paraRemover = tercEstList.Where(x => xml.lotacoes.Any(y => y.sigla == x.sigla));
                foreach (var item in paraRemover)
                {
                    tercEstList.Remove(item);
                }

                var tercEst = tercEstList.ToArray();
                if (tercEst != null && tercEst.Length > 0)
                    pess.AddRange(tercEst);
                //*/

                // Converte a lista em array e adiciona resultado ao arquivo
                xml.pessoas = pess.ToArray();

                if (pess == null || pess.Count <= 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Carga de {0} não criada porque não há pessoas", dep.siglaSiga);
                    continue;
                }


                empregadosInseridos.AddRange(pess);
                Console.WriteLine("OK!");

                String nomeArquivo = String.Format("CargaSiga{0}.xml", dep.siglaSiga);
                //Console.WriteLine(String.Format("Serializando XML e escrevendo arquivo {0}{1}... ", path, nomeArquivo));
                //XmlSerializer serializer = new XmlSerializer(typeof(@base));
                //StreamWriter arq = File.CreateText(String.Concat(path, nomeArquivo));

                //serializer.Serialize(arq, xml);

                // Aplicativo importador da carga - Siga versão 8
                //script.WriteLine("\"%JAVA_HOME_1_7%\\java.exe\" -Xms600m -Xmx600m -Dsiga.properties.file=.\\consumidor_xml\\%CONSUMIDOR_CONFI% -jar .\\consumidor_xml\\siga-cp-sinc.one-jar.jar -desenv -url=\"file:///%XML_FOLDER%/{0}\" 1> %XML_FOLDER%\\logs\\{0}.log 2>&1", nomeArquivo);
                //script.Flush();

                // Carga por webservice - Siga Versão 10
                //script.WriteLine("curl -H \"Authorization: 1c13bd14-483d-45ad-9b4d-2b9e78414cfb\" -k -F file=@%XML_FOLDER%\\{0} \"https://rancher.infraero.gov.br:32249/siga/public/app/admin/sinc?sigla={1}&maxSinc=3000&modoLog=false\" 1> %XML_FOLDER%\\logs\\{0}.log 2>&1", nomeArquivo, dep.siglaSiga);
                // script.Flush();

                Console.WriteLine(String.Format("Finalizado para a dependência {0}/{1}!", dep.codigo, dep.siglaSiga));
                Console.WriteLine(""); ;

                XmlSerializer serializer = new XmlSerializer(typeof(@base));
                StringWriter stringWriter = new StringWriter();
                //serializer.Serialize(stringWriter, xml);

                using (var memoryStream = new MemoryStream())
                using (var xmlWriter = new StreamWriter(memoryStream, Encoding.UTF8))
                {
                    // Serializar o objeto XML diretamente para o StreamWriter
                    serializer.Serialize(xmlWriter, xml);
                    xmlWriter.Flush(); // Certifique-se de que todos os dados foram gravados no MemoryStream

                    // Converta o MemoryStream para byte array
                    var byteArray = memoryStream.ToArray();
                    var streamContent = new StreamContent(new MemoryStream(byteArray));
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                    streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "file",
                        FileName = nomeArquivo
                    };

                    // Configura o HttpClient e prepara a requisição
                    HttpClientHandler handler = new HttpClientHandler();
                    handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", "1c13bd14-483d-45ad-9b4d-2b9e78414cfb"); // Inclua "Bearer" se for um token Bearer

                        // Cria o conteúdo da requisição com o XML
                        MultipartFormDataContent content = new MultipartFormDataContent();
                        content.Add(streamContent, "file", "file.xml"); // O segundo parâmetro deve ser o nome do campo esperado pelo endpoint
                        string urlEndPointSigaDocVar = Environment.GetEnvironmentVariable("URL_ENDPOINT_SIGADOC");
                        string parametros = "/siga/public/app/admin/sinc?sigla=" + dep.siglaSiga  + "&maxSinc=3000&modoLog=false";

                        string urlEndPointSigaDoc = urlEndPointSigaDocVar + parametros;
                        // Envia o XML para o endpoint
                        HttpResponseMessage response = null;
                         
                        try
                        {
                            if (urlEndPointSigaDoc != null)
                            {
                                response = await client.PostAsync(urlEndPointSigaDoc, content);
                                Console.WriteLine("End-point: " + urlEndPointSigaDoc);
                             }
                            else {
                                Console.WriteLine("urlEndPointSigaDoc está null");
                            }

                        }
                        catch (HttpRequestException ex)
                        {
                            // Tratar casos onde o servidor está fora ou outras falhas de rede
                            Console.WriteLine($"Erro ao conectar ao servidor: {ex.Message}");
                            // Aqui você pode implementar uma lógica de retry, log, etc.
                        }
                        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
                        {
                            // Tratar casos de timeout
                            Console.WriteLine("A requisição expirou.");
                        }
                        catch (Exception ex)
                        {
                            // Tratar outras exceções inesperadas
                            Console.WriteLine($"Ocorreu um erro inesperado: {ex.Message}");
                        }
                        finally
                        {
                            if (response != null)
                            {
                                Console.Write(response.Content.ReadAsStringAsync().Result);
                                response.Dispose();
                               
                            }
                        }

                        //string responseContent = await response.Content.ReadAsStringAsync(); // Obtenha o conteúdo da resposta para depuração



                    }
                }

                //File.Delete(path + "\\*.*");

               // StreamWriter lista = File.CreateText(String.Concat(path, "naoInseridos.txt"));
                foreach (var mat in daoGestorh.getEmpregadosAtivos())
                {
                    //if (empregadosInseridos.Find(x => (x.matricula == mat)) == null)
                       // lista.WriteLine(mat);
                }
               // lista.Flush();
            }

        }
    }
}
