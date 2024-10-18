using GerenciaEmail.Infra.Services;
using GerenciaEmail.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SigaDocIntegracao.Web.Models.ModuloEmail;
using SigaDocIntegracao.Web.Persistence;
using SigaDocIntegracao.Web.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading;
using System.Threading.Tasks;


namespace SigaDocIntegracao.Web.Tasks
{
    public class TaskModuloEmail : IHostedService, IDisposable
    {
        private readonly ILogger<TaskModuloEmail> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer;
        private int Intervalo;

        public TaskModuloEmail(ILogger<TaskModuloEmail> logger, IConfiguration config, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            Intervalo = int.TryParse(Environment.GetEnvironmentVariable("INTERVALO_EMAIL"), out var intervalo) ? intervalo : 10;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Serviço iniciado.");
            //_timer = new Timer(ExecuteTask, null, TimeSpan.Zero, TimeSpan.FromMinutes(Intervalo));
            return Task.CompletedTask;
        }

        private async void ExecuteTask(object state)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<Contexto>();
                    var emailService = new EmailService(context);
                    var mailService = new MailService();
                    String[] emails = new String[1];
                    emails[0] = "t031842941@infraero.gov.br";

                    List<ExModeloEmailParamModel> exModeloEmailParamModelsList = await emailService.GetModelosCadastradosAsync();

                    List<string> idModelosList = exModeloEmailParamModelsList
                                                          .Select(model => model.IdSigaDoc)
                                                        .ToList();
                    List<ExDocAssinadoModel> documentosAssinadosList = new List<ExDocAssinadoModel>();
                    foreach (ExModeloEmailParamModel modelo in exModeloEmailParamModelsList)
                    {

                        documentosAssinadosList.AddRange(emailService.GetDocumentosAssinados(modelo));

                        if (modelo.Ativo) {
                            modelo.DataUltimoProcessamento = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                            context.Update(modelo);
                          
                            await context.SaveChangesAsync();
                        }
                    }

                    List<DocAssinadoEmailHistoricoModel> docAssinadoEmailHistoricoModelList = new List<DocAssinadoEmailHistoricoModel>();
                    docAssinadoEmailHistoricoModelList = await emailService.GetStatusDocumentosAsync();
                    List<DocAssinadoEmailHistoricoModel> docAssinadoEmailHistoricoModelListAtual = new List<DocAssinadoEmailHistoricoModel>();

                    Boolean enviado = false;
                    if (exModeloEmailParamModelsList.Count != 0)
                    {
                        foreach (ExDocAssinadoModel docAssinado in documentosAssinadosList)
                        {
                            var documentosEnviados = docAssinadoEmailHistoricoModelList
                               .Where(model => model.Enviado)
                               .ToDictionary(model => model.Documento, model => true);

                            if (documentosEnviados.ContainsKey(docAssinado.CodigoDocumento))
                            {
                                continue;
                            }

                            foreach (ExModeloEmailParamModel modelo in exModeloEmailParamModelsList)
                            {
                                if (modelo.DescricaoModelo == docAssinado.ModeloDocumento && modelo.Ativo == true)
                                {
                                    Thread.Sleep(5000);
                                    docAssinadoEmailHistoricoModelListAtual = await emailService.GetStatusDocumentosAsync();

                                    var documentosEnviadosAtual = docAssinadoEmailHistoricoModelListAtual
                                     .Where(model => model.Enviado)
                                    .ToDictionary(model => model.Documento, model => true);

                                    var documentosEnviadosAtual2 = docAssinadoEmailHistoricoModelListAtual
                                    .ToDictionary(model => model.Documento);

                                    if (documentosEnviadosAtual.ContainsKey(docAssinado.CodigoDocumento))
                                    {
                                        continue;
                                    }

                                    DocAssinadoEmailHistoricoModel modelExistente = null;
                                    if (documentosEnviadosAtual2.ContainsKey(docAssinado.CodigoDocumento))
                                    {
                                        modelExistente = documentosEnviadosAtual2[docAssinado.CodigoDocumento];
                                        enviado = true;
                                    }

                                    SendMailViewModel sendMailViewModel = new SendMailViewModel
                                    {
                                        Emails = modelo.Destinatarios
                                            .Split(';')
                                            .Select(email => email.Trim())
                                            .Where(trimmedEmail => !string.IsNullOrEmpty(trimmedEmail))
                                            .ToArray(),
                                        Subject = modelo.Assunto,
                                        Body = modelo.ConteudoEmail,
                                        IsHtml = true
                                    };

                                    mailService.SendMail(docAssinado.CodigoDocumento, sendMailViewModel.Emails, sendMailViewModel.Subject, sendMailViewModel.Body, sendMailViewModel.IsHtml);

                                    DocAssinadoEmailHistoricoModel docAssinadoEmailHistoricoModel;
                                    if (modelExistente != null)
                                    {
                                        docAssinadoEmailHistoricoModel = modelExistente;
                                        docAssinadoEmailHistoricoModel.Enviado = true;
                                    }
                                    else
                                    {
                                        docAssinadoEmailHistoricoModel = new DocAssinadoEmailHistoricoModel
                                        {
                                            Documento = docAssinado.CodigoDocumento,
                                            Enviado = true
                                        };
                                    }

                                    if (!enviado)
                                    {
                                        emailService.salvarDocAssinadoEmailHistoricoModel(docAssinadoEmailHistoricoModel);
                                    }
                                    else
                                    {
                                        emailService.alterarDocAssinadoEmailHistoricoModel(docAssinadoEmailHistoricoModel);
                                        enviado = false;
                                    }

                                    documentosEnviados[docAssinado.CodigoDocumento] = true;
                                }
                            }
                        }
                    }
              
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar o GeradorXMLSigaDoc");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Serviço parado.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public void ReiniciarTimer(int IntervaloAtual)
        {
            _timer.Change(TimeSpan.Zero, TimeSpan.FromMinutes(IntervaloAtual));
        }
    }
}
