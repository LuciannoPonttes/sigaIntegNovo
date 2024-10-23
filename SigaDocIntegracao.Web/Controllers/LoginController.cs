using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SigaDocIntegracao.Web.Persistence;
using SigaDocIntegracao.Web.UsuarioContexto.Models;
using SigaDocIntegracao.Web.UsuarioContexto.Services;
using SigaDocIntegracao.Web.UsuarioContexto.ViewModel;
using System.Security.Claims;

namespace SigaDocIntegracao.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly UsuarioService _usuarioService;

        public LoginController(Contexto contexto)
        {
            _usuarioService = new UsuarioService(contexto);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            var autenticado = Autenticacao(viewModel.Matricula, viewModel.Senha);

            if (autenticado)
            {
                var usuario = await _usuarioService.BuscarPorMatricula(viewModel.Matricula);

                if (usuario is not null)
                {
                    var claims = new List<Claim>()
                    {
                        new(ClaimTypes.Name, usuario.Nome),
                        new(ClaimTypes.Email, usuario.Email),
                    };

                    if (usuario.Tipo.Equals(Tipo.Admin))
                    {
                        claims.Add(new Claim("Tipo", "Admin"));
                        claims.Add(new Claim(ClaimTypes.Role, usuario.Tipo.Equals(Tipo.Admin) ? "Admin" : "Comum"));
                    }

                    var permissoes = await _usuarioService.BuscarPermissoesUsuario(usuario.Id);

                    if (permissoes is not null && permissoes.Count > 0)
                    {
                        var permissoesString = string.Join(",", permissoes.Select(p => p.Nome.Trim()));
                        claims.Add(new Claim("Permissao", permissoesString));
                    }

                    ClaimsIdentity claimsIdentity = new(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new(claimsIdentity);

                    await HttpContext.SignInAsync(claimsPrincipal);

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Email ou senha inválidos");

            return View("Index", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            HttpContext.User = null;
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            Response.Cookies.Delete(".AspNetCore.Cookies");

            return RedirectToAction("Index", "Login");
        }


        [HttpGet("/AcessoNegado")] 
        public IActionResult AcessoNegado()
        {
            return View();
        }

        private bool Autenticacao(string matricula, string senha)
        {
            return true;
        }

        
    }
}