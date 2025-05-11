using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Курсач_1.Models;
using Курсач_1.Repositories;

namespace Курсач_1.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UsersControllers(UserRepositories repositories) : ControllerBase
    {
        static string secretKey = "super_super_secret_key_with_256_bits_!!!"; // 256+ бит
        static string issuer = "MyApp";
        static string audience = "MyUsers";
        static SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        string GenerateToken(string username, int id, int minutes)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(minutes),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("/registr")]
        public async Task<IActionResult> Registration([FromBody] CreateUserRequest User)
        {
            bool result = await repositories.Add(User.UserName, User.Password, User.Email);
            return result ? Ok("Регистрация прошла успешно") : Unauthorized("Пользователь с таким именем существует");
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {

            User user = await repositories.GetUser(request.UserName, request.Password);

            if(user == null)
                return Unauthorized();

            var accessToken = GenerateToken(user.UserName, user.Id, 1);      // 1 минута
            var refreshToken = GenerateToken(user.UserName, user.Id, 60);    // 60 минут

            Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddMinutes(1)
            });

            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddMinutes(60)
            });

            return Ok("Авторизация успешна");

        }

        [HttpGet("/logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");
            return Ok("Вы вышли");
        }

        

    }

    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class CreateUserRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
