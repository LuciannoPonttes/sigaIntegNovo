
namespace GerenciaEmail.Infra.Services
{
    public interface IMailService
    {
        void SendMail(string doc, string[] emails, string subject, string body, bool isHtml = false);
    }
}
