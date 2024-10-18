using CargaSigaDoc;
namespace SigaDocIntegracao.Web.Tasks
{
    public class TaskModuloCarga : IHostedService, IDisposable
    {
        private readonly ILogger<TaskModuloCarga> _logger;
        private Timer _timer;
        private int Intervalo;
        //commit
        public TaskModuloCarga(ILogger<TaskModuloCarga> logger)
        {
            _logger = logger;
            Intervalo = int.TryParse(Environment.GetEnvironmentVariable("INTERVALO_EMAIL"), out var intervalo) ? intervalo : 60;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Serviço iniciado.");
            _timer = new Timer(ExecuteTask, null, TimeSpan.Zero, TimeSpan.FromMinutes(Intervalo));
            return Task.CompletedTask;
        }

        private void ExecuteTask(object state)
        {
            try
            {
                CargaService program2 = new CargaService();
                //program2.ExecutaXml(); -- DESATIVADO
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
