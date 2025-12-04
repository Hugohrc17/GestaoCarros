using Microsoft.AspNetCore.Identity;
using GestãoCarros.Models.Dtos;
using GestãoCarros.Models;
using ConcessionariaModel = GestãoCarros.Models.Concessionaria;
using AutoMapper;
using ImobiFlow.Api.Core.Interfaces;

namespace GestãoCarros.Services.Usuarios
{
    public class UsuarioService
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IMapper _mapper;
        private readonly IRepositorioGenerico _repositorioGenerico;

        public UsuarioService(UserManager<Usuario> userManager, RoleManager<IdentityRole<Guid>> roleManager, IMapper mapper,
            IRepositorioGenerico repositorioGenerico)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _repositorioGenerico = repositorioGenerico;
        }

        public async Task<IdentityResult> CriarUsuarioInternoAsync(UsuarioDto usuarioDto, string role = null!)
        {
            var usuario = new Usuario
            {
                UserName = usuarioDto.Email,
                Email = usuarioDto.Email,
                Nome = usuarioDto.Nome,

                Ativo = true
            };

            var result = await _userManager.CreateAsync(usuario, usuarioDto.Senha!);

            return result;
        }

        public async Task<IdentityResult> CriarUsuarioAsync(UsuarioDto usuarioDto, string role = null!)
        {
            // Validar se a concessionária existe
            var concessionaria = await _repositorioGenerico.ObterPorIdAsync<ConcessionariaModel>(usuarioDto.ConcessionariaId);
            if (concessionaria == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Concessionária não encontrada." });
            }

            // Verificar se email já existe
            var usuarioExistente = await _userManager.FindByEmailAsync(usuarioDto.Email!);
            if (usuarioExistente != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Este email já está registrado." });
            }

            var usuario = new Usuario
            {
                UserName = usuarioDto.Email,
                Email = usuarioDto.Email,
                Nome = usuarioDto.Nome,
                ConcessionariaId = usuarioDto.ConcessionariaId,
                Ativo = true
            };

            var result = await _userManager.CreateAsync(usuario, usuarioDto.Senha!);

            if (result.Succeeded && !string.IsNullOrEmpty(role))
            {
                var normalizedRole = role.ToUpper();
                if (!await _roleManager.RoleExistsAsync(normalizedRole))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>(normalizedRole));
                }
                await _userManager.AddToRoleAsync(usuario, normalizedRole);
            }

            return result;
        }

        public async Task<IdentityResult> CriarRoleAsync(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{roleName}' já existe." });

            var result = await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            return result;
        }

        public async Task<IdentityResult> AdicionarUsuarioRoleAsync(string userId, string roleName)
        {
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
                return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado." });

            if (!await _roleManager.RoleExistsAsync(roleName))
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{roleName}' não existe." });

            var result = await _userManager.AddToRoleAsync(usuario, roleName);
            return result;
        }

        public async Task<Usuario?> AutenticarUsuarioAsync(string email, string senha)
        {
            var usuario = await _userManager.FindByEmailAsync(email);
            if (usuario != null && await _userManager.CheckPasswordAsync(usuario, senha))
                return usuario;
            return null;
        }

        public async Task<Usuario?> ObterPorIdAsync(Guid Usuarioid)
        {
            var usuario = await _repositorioGenerico.ObterPorIdAsync<Usuario>(Usuarioid)
             ?? throw new Exception("Usuário não encontrado, tente novamente");
            return usuario;
        }

        public async Task<IEnumerable<UsuarioDto>> ObterTodosAsync()
        {
            var todosUsuarios = await _repositorioGenerico.ObterTodosAsync<Usuario>(u => true)
             ?? throw new Exception("Erro ao obter todos usuários");

            var usuariosDto = _mapper.Map<IEnumerable<UsuarioDto>>(todosUsuarios);
            return usuariosDto;
        }

        public async Task AtualizarAsync(Guid id, Usuario usuario)
        {
            var Usuario = await _repositorioGenerico.ObterTodosPorIdAsync<Usuario>(id)
                ?? throw new Exception("Usuario não existe!");

            await _repositorioGenerico.AtualizarAsync(usuario);
        }

        public async Task ExcluirAsync(Guid usuarioId)
        {
            await _repositorioGenerico.ExcluirAsync<Usuario>(usuarioId);
        }
    }
}