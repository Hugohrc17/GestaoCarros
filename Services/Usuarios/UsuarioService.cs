using Microsoft.AspNetCore.Identity;
using GestãoCarros.Models.Dtos;
using GestãoCarros.Models;

namespace GestãoCarros.Services.Usuarios
{
    public class UsuarioService
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public UsuarioService(UserManager<Usuario> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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
    }
}