using AutoMapper;
using GestãoCarros.Models;
using GestãoCarros.Models.Dtos;
using GestãoCarros.Services.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestãoCarros.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,AdminInterno")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;
        private readonly UserManager<Usuario> _userManager;
        private readonly IMapper _mapper;

        public UsuarioController(UsuarioService usuarioService,UserManager<Usuario> userManager, IMapper mapper)
        {
            _usuarioService = usuarioService;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpPost("criar-usuario")]
        public async Task<IActionResult> CriarUsuario([FromBody] UsuarioDto usuarioDto, [FromQuery] string? role = null)
        {
            var result = await _usuarioService.CriarUsuarioAsync(usuarioDto, role!);
            if (result.Succeeded)
                return Ok("Usuário criado com sucesso.");
            return BadRequest(result.Errors);
        }

        [HttpPost("criar-usuario-interno")]
        public async Task<IActionResult> CriarUsuarioInterno([FromBody] UsuarioDto usuarioDto, [FromQuery] string? role = null)
        {
            var result = await _usuarioService.CriarUsuarioInternoAsync(usuarioDto, role!);
            if (result.Succeeded)
                return Ok("Usuário criado com sucesso.");
            return BadRequest(result.Errors);
        }
        
        [HttpPost("criar-role")]
        public async Task<IActionResult> CriarRole([FromQuery] string roleName)
        {
            var result = await _usuarioService.CriarRoleAsync(roleName);
            if (result.Succeeded)
                return Ok($"Role '{roleName}' criada com sucesso.");
            return BadRequest(result.Errors);
        }

        [HttpPost("adicionar-role")] 
        public async Task<IActionResult> AdicionarUsuarioRole([FromQuery] string userId, [FromQuery] string roleName)
        {
            var result = await _usuarioService.AdicionarUsuarioRoleAsync(userId, roleName);
            if (result.Succeeded)
                return Ok($"Usuário adicionado à role '{roleName}' com sucesso.");
            return BadRequest(result.Errors);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsuarioDto usuarioDto)
        {
            var usuario = await _usuarioService.AutenticarUsuarioAsync(usuarioDto.Email!, usuarioDto.Senha!);
            if (usuario == null)
                return Unauthorized("Usuário ou senha inválidos.");

            var roles = await _userManager.GetRolesAsync(usuario);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email ?? ""),
                new Claim("ConcessionariaId", usuario.ConcessionariaId.ToString())
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKey@2025"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                usuarioId = usuario.Id,
                email = usuario.Email,
                concessionariaId = usuario.ConcessionariaId,
                roles = roles
            });
        }
        
        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var usuario = await _usuarioService.ObterTodosAsync();
            return Ok(usuario);
        }

        [HttpGet ("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(Guid id, UsuarioDto usuarioDto)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            var usuarioAtualizado = _mapper.Map(usuarioDto, usuario);
            await _usuarioService.AtualizarAsync(id, usuarioAtualizado);
            return Ok(usuarioAtualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(Guid id)
        {
            var usuario = await _usuarioService.ObterPorIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            await _usuarioService.ExcluirAsync(id);
            return NoContent();
        }
    }
}
