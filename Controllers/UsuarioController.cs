using GestãoCarros.Models;
using GestãoCarros.Models.Dtos;
using GestãoCarros.Services.Usuarios;
using Microsoft.AspNetCore.Authentication;
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
    // [Authorize(Roles = "Admin,AdminInterno")]
    public class UsuarioController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly UserManager<Usuario> _userManager;

        public UsuarioController(UsuarioService usuarioService,UserManager<Usuario> userManager)
        {
            _usuarioService = usuarioService;
            _userManager = userManager;
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

        // Login MVC (GET)
        [HttpGet]
        [Route("/Usuario/Login")]
        [AllowAnonymous]
        public IActionResult LoginView()
        {
            return View("~/Views/Usuario/Login.cshtml");
        }

        // Login MVC (POST)
        [HttpPost]
        [Route("/Usuario/Login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginView(UsuarioDto usuarioDto)
        {
            var usuario = await _usuarioService.AutenticarUsuarioAsync(usuarioDto.Email!, usuarioDto.Senha!);
            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuário ou senha inválidos.");
                return View("~/Views/Usuario/Login.cshtml", usuarioDto);
            }

            // Autenticação de sessão/cookie padrão MVC
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Email ?? "")
            };
            var claimsIdentity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            await HttpContext.SignInAsync(
                IdentityConstants.ApplicationScheme,
                new ClaimsPrincipal(claimsIdentity)
            );

            return RedirectToAction("Dashboard", "Home");
        }

        // Página de login AJAX (GET)
        [HttpGet]
        [Route("/Usuario/LoginToken")]
        [AllowAnonymous]
        public IActionResult LoginToken()
        {
            return View("~/Views/Usuario/LoginToken.cshtml");
        }

        // Página protegida para teste do token
        [HttpGet]
        [Route("/Usuario/TesteToken")]
        public IActionResult TesteToken()
        {
            return Content("Token JWT válido! Usuário autenticado.");
        }
    }
}
