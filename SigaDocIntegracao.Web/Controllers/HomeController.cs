using GerenciaEmail.Infra.Services;
using GerenciaEmail.ViewModels;
using Microsoft.AspNetCore.Mvc;
using SigaDocIntegracao.Web.Models;
using System.Diagnostics;

namespace SigaDocIntegracao.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
      
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            
        }

        public IActionResult Index()
        {
            /*
            String[] emails = new String[1]; // Inicializa o array com o tamanho desejado

            emails[0] = "t031842941@infraero.gov.br"; // Atribui o valor ao índice 1

            MailService _mailService = new MailService();

            SendMailViewModel sendMailViewModel = new SendMailViewModel();
            sendMailViewModel.Emails = emails;

            sendMailViewModel.Subject = "teste em dsv pelo controller";
            sendMailViewModel.Body = "2315";
            sendMailViewModel.IsHtml = true;
            _mailService.SendMail(sendMailViewModel.Emails, sendMailViewModel.Subject, sendMailViewModel.Body,
            sendMailViewModel.IsHtml);

            */

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EnviarEmailPost([FromBody] SendMailViewModel sendMailViewModel)
        {
            //_mailService.SendMail(sendMailViewModel.Emails, sendMailViewModel.Subject, sendMailViewModel.Body,
            //   sendMailViewModel.IsHtml);

           return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
