// No seu arquivo Controllers/AuthController.cs

using APITATT1.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration; // Para acessar as configurações do appsettings
using System.Threading.Tasks;
using BCrypt.Net; // Use BCrypt.Net para o hash de senha
using System.ComponentModel.DataAnnotations;

namespace APITATT1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Contexto _context;
        private readonly IConfiguration _configuration;

        public AuthController(Contexto context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // DTO para registrar um novo usuário de API (OPCIONAL: se você quiser um endpoint de registro)
        // CUIDADO: Este endpoint permitiria qualquer um criar um usuário da API.
        // Em produção, você provavelmente vai querer criar este usuário manualmente ou de forma segura.
        public class RegisterUserDto
        {
            [Required]
            public string Username { get; set; }
            [Required]
            [MinLength(6)] // Senha mínima de 6 caracteres
            public string Password { get; set; }
        }

        // DTO para login
        public class LoginDto
        {
            [Required]
            public string Username { get; set; }
            [Required]
            public string Password { get; set; }
        }


        // POST: api/Auth/Register (OPCIONAL)
        // Se você não quiser que usuários gerais sejam registrados via API, pode remover este método.
        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register(RegisterUserDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest("Usuário já existe.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password); // Cria o hash da senha

            var newUser = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return StatusCode(201, "Usuário geral da API cadastrado com sucesso!");
        }


        // POST: api/Auth/Login
        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(LoginDto request)
        {
            // 1. Encontrar o usuário geral pelo Username
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
            {
                return BadRequest("Credenciais inválidas."); // Não diga qual está errado por segurança
            }

            // 2. Verificar a senha (comparar o hash da senha informada com o hash salvo)
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("Credenciais inválidas.");
            }

            // 3. Se a senha estiver correta, gerar o token JWT
            string token = CreateToken(user);

            return Ok(new { token }); // Retorna o token em um objeto JSON
        }

        // Método para criar o token JWT
        private string CreateToken(User user)
        {
            // Claims são como "certificados" dentro do token, com informações sobre o usuário
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // ID do usuário
                new Claim(ClaimTypes.Name, user.Username) // Username do usuário
                // Você pode adicionar mais claims aqui, como roles (roles) se tiver
            };

            // Pegar a chave secreta do appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Criar a descrição do token (o que ele vai conter)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7), // Token expira em 7 dias (ajuste como quiser)
                SigningCredentials = creds,
                Issuer = _configuration["Jwt:Issuer"],     // Quem emitiu
                Audience = _configuration["Jwt:Audience"]  // Para quem é
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token); // Converte o token em uma string
        }
    }
}