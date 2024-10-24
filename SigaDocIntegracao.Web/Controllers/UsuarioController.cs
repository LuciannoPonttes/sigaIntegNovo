using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SigaDocIntegracao.Web.Persistence;
using SigaDocIntegracao.Web.UsuarioContexto.Models;
using SigaDocIntegracao.Web.UsuarioContexto.Services;
using SigaDocIntegracao.Web.UsuarioContexto.ViewModel;

namespace SigaDocIntegracao.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsuarioController : Controller
    {
        private readonly UsuarioService _usuarioService;

        public UsuarioController(Contexto contexto)
        {
            _usuarioService = new UsuarioService(contexto);
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pageIndex = 0)
        {
            var usuarios = await _usuarioService.BuscarPaginadoAsync();

            if (usuarios.TotalCount == 0)
            {
                return View();
            }

            return View(usuarios);
        }

        [HttpGet]
        public async Task<IActionResult> IndexFilter(string query, StatusCadastro? status = null, int pageIndex = 0)
        {
            var usuarios = await _usuarioService.BuscarPaginadoAsync(query, status, pageIndex);

            return PartialView("_TabelaUsuarios", usuarios);
        }

        [HttpGet]
        public async Task<IActionResult> Adicionar()
        {
            var permissoesDisponiveis = await _usuarioService.BuscarPermissoesDisponiveisAsync();

            var usuario = new UsuarioViewModel()
            {
                PermissoesDisponiveis = permissoesDisponiveis,
            };

            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Adicionar(UsuarioViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _usuarioService.AdicionarAsync(viewModel);

                if (result is null)
                {
                    var permissoesDisponiveis = await _usuarioService.BuscarPermissoesDisponiveisAsync();
                    TempData["ErrorMessage"] = $"A matrícula {viewModel.Matricula} já foi cadastrada.";
                    viewModel.PermissoesDisponiveis = permissoesDisponiveis;
                    return View(viewModel);
                }

                return RedirectToAction("Index");
            }

            return View(ModelState);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(Guid id)
        {
            var usuario = await _usuarioService.BuscarPorIdAsync(id);
            var permissoesDisponiveis = await _usuarioService.BuscarPermissoesDisponiveisAsync();
            var permissoesSelecionadasIds = await _usuarioService.BuscarPermissoesIdUsuario(id);

            AtualizarUsuarioViewModel viewModel = new()
            {
                Id = id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Matricula = usuario.Matricula,
                Tipo = usuario.Tipo,
                StatusCadastro = usuario.StatusCadastro,
                PermissoesDisponiveis = permissoesDisponiveis,
                PermissoesSelecionadas = (permissoesSelecionadasIds.Any()) ? permissoesSelecionadasIds : new List<Guid>()
            };

            return View(viewModel); 
        }

        [HttpPost]
        [ActionName("Editar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(AtualizarUsuarioViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                await _usuarioService.AtualizarAsync(viewModel);

                return RedirectToAction("Index");
            }

            return View(ModelState);
        }

        [HttpPost]
        [ActionName("Excluir")]
        public async Task<IActionResult> Excluir(Guid id)
        {
            await _usuarioService.ExcluirAsync(id);

            return RedirectToAction("Index");
        }

    }
}
