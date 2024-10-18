using Microsoft.EntityFrameworkCore;
using SigaDocIntegracao.Web.Extensions;
using SigaDocIntegracao.Web.Persistence;
using SigaDocIntegracao.Web.UsuarioContexto.Models;
using SigaDocIntegracao.Web.UsuarioContexto.ViewModel;

namespace SigaDocIntegracao.Web.UsuarioContexto.Services
{
    public class UsuarioService
    {
        private readonly Contexto _contexto;

        public UsuarioService(Contexto contexto)
        {
            _contexto = contexto;
        }

        public async Task<ListarUsuarioPaginadoViewModel> BuscarPaginadoAsync(string search = null,
            StatusCadastro? statusCadastro = null,
            int pageIndex = 0,
            int pageSize = 10)
        {
            var usuariosQuery = _contexto.Usuario.AsNoTracking().OrderBy(u => u.Nome).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                var searchFormatadoSemCaracteresEspeciais = search.RemoveCaracteresEspeciais();
                var searchMaiusculo = search.ToUpper();
                usuariosQuery = usuariosQuery.Where(u => u.NomeNormalizado.Contains(searchFormatadoSemCaracteresEspeciais) ||
                    u.Email.ToUpper().Contains(searchMaiusculo));
            }

            if (statusCadastro != null)
            {
                usuariosQuery = usuariosQuery.Where(u => u.StatusCadastro.Equals(statusCadastro));
            }

            var usuarios = await usuariosQuery.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();

            var totalUsuariosQuery = _contexto.Usuario.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                var searchFormatadoSemCaracteresEspeciais = search.RemoveCaracteresEspeciais();
                var searchMaiusculo = search.ToUpper();

                totalUsuariosQuery = totalUsuariosQuery.Where(u => u.NomeNormalizado.Contains(searchFormatadoSemCaracteresEspeciais) ||
                    u.Email.ToUpper().Contains(searchMaiusculo));
            }

            if (statusCadastro != null)
            {
                totalUsuariosQuery = totalUsuariosQuery.Where(u => u.StatusCadastro.Equals(statusCadastro));
            }

            var totalUsuarios = await totalUsuariosQuery.CountAsync();

            return new ListarUsuarioPaginadoViewModel
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalUsuarios,
                Usuarios = usuarios,
                TotalPages = (int)Math.Ceiling((double)totalUsuarios / pageSize)
            };
        }


        public async Task<Usuario> BuscarPorIdAsync(Guid id)
        {
            return await _contexto.Usuario.FirstOrDefaultAsync(u => u.Id.Equals(id));
        }

        public async Task<Usuario> BuscarPorEmail(string email)
        {
            return await _contexto.Usuario.FirstOrDefaultAsync(u => u.Email.Equals(email));
        }

        public async Task<Usuario> BuscarPorMatricula(string matricula)
        {
            return await _contexto.Usuario.FirstOrDefaultAsync(u => u.Matricula.Equals(matricula));
        }

        public async Task<Usuario> AdicionarAsync(UsuarioViewModel viewModel)
        {
            var usuarioExiste = await BuscarPorMatricula(viewModel.Matricula);

            if (usuarioExiste != null) return null;

            var usuario = Usuario.CriarUsuario(viewModel.Nome, viewModel.Email, viewModel.Tipo, viewModel.Matricula);
            _contexto.Add(usuario);

            if (viewModel.PermissoesSelecionadas.Any())
            {
                foreach (var permissaoId in viewModel.PermissoesSelecionadas)
                {
                    var usuarioPermissao = new UsuarioPermissao(usuario.Id, permissaoId);
                    _contexto.UsuarioPermissao.Add(usuarioPermissao);
                }
            }

            await SaveChangesAsync();

            return usuario;
        }

        public async Task<Usuario> AtualizarAsync(AtualizarUsuarioViewModel viewModel)
        {
            var usuario = await BuscarPorMatricula(viewModel.Matricula);

            usuario.Editar(viewModel.Nome, viewModel.Email, viewModel.Tipo, (StatusCadastro)viewModel.StatusCadastro);
            _contexto.Update(usuario);


            var permissoesAntigas = await BuscarPermissoesIdUsuario(usuario.Id);

            foreach (var permissaoId in permissoesAntigas)
            {
                if (viewModel.PermissoesSelecionadas != null && !viewModel.PermissoesSelecionadas.Contains(permissaoId))
                {
                    var permissaoUsuario = await BuscarPermissaoUsuario(usuario.Id, permissaoId);
                    _contexto.UsuarioPermissao.Remove(permissaoUsuario);
                }

                if (viewModel.PermissoesSelecionadas is null)
                {
                    var permissaoUsuario = await BuscarPermissaoUsuario(usuario.Id, permissaoId);
                    _contexto.UsuarioPermissao.Remove(permissaoUsuario);
                }
            }

            if (viewModel.PermissoesSelecionadas != null && viewModel.PermissoesSelecionadas.Any())
            {
                foreach (var permissaoId in viewModel.PermissoesSelecionadas)
                {          
                    if (!permissoesAntigas.Contains(permissaoId))
                    {
                        var usuarioPermissao = new UsuarioPermissao(usuario.Id, permissaoId);
                        _contexto.UsuarioPermissao.Add(usuarioPermissao);
                    }                    
                }
            }

            await SaveChangesAsync();

            return usuario;
        }

        public async Task<bool> ExcluirAsync(Guid usuarioId)
        {
            var usuario = await BuscarPorIdAsync(usuarioId);

            _contexto.Usuario.Remove(usuario);
            await SaveChangesAsync();

            return true;
        }

        public async Task<List<Permissao>> BuscarPermissoesDisponiveisAsync()
        {
            return await _contexto.Permissao.ToListAsync();
        }

        public async Task<List<Guid>> BuscarPermissoesIdUsuario(Guid usuarioId)
        {
            return await _contexto.UsuarioPermissao.Where(up => up.UsuarioId.Equals(usuarioId))
                .Select(up => up.PermissaoId)
                .ToListAsync();
        }

        public async Task<UsuarioPermissao> BuscarPermissaoUsuario(Guid usuarioId, Guid permissaoId)
        {
            return await _contexto.UsuarioPermissao.FirstOrDefaultAsync(up => 
                up.UsuarioId.Equals(usuarioId) && 
                up.PermissaoId.Equals(permissaoId));
        }

        public async Task<Permissao> BuscarPermissao(Guid permissaoId)
        {
            return await _contexto.Permissao.FirstOrDefaultAsync(p => p.Id.Equals(permissaoId));
        }

        public async Task<List<Permissao>> BuscarPermissoesUsuario(Guid usuarioId)
        {
            var usuarioPermissoesIds = await BuscarPermissoesIdUsuario(usuarioId);

            if (usuarioPermissoesIds != null && usuarioPermissoesIds.Count > 0)
            {
                List<Permissao> permissoes = new();

                foreach (var permissaoId in usuarioPermissoesIds)
                {
                    var permissao = await BuscarPermissao(permissaoId);
                    permissoes.Add(permissao);
                }

                return permissoes;
            }

            return null;
        }

        private async Task<int> SaveChangesAsync()
        {
            return await _contexto.SaveChangesAsync();
        }

    }
}
