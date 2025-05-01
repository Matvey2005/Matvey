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
            bool result = await repositories.Add(User.UserName, User.Password);
            return result ? Ok("Регистрация прошла успешно") : Unauthorized("Пользователь с таким именем существует");
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody]CreateUserRequest User)
        {

            User user = await repositories.GetUser(User.UserName, User.Password);

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

        //public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        //{
        //    // Другие middleware...

        //    app.Use(async (context, next) =>
        //    {
        //        var handler = new JwtSecurityTokenHandler();
        //        var accessToken = context.Request.Cookies["access_token"];
        //        var refreshToken = context.Request.Cookies["refresh_token"];

        //        ClaimsPrincipal? principal = null;

        //        if (!string.IsNullOrEmpty(accessToken))
        //        {
        //            try
        //            {
        //                principal = handler.ValidateToken(accessToken, new TokenValidationParameters
        //                {
        //                    ValidateIssuer = true,
        //                    ValidIssuer = issuer,
        //                    ValidateAudience = true,
        //                    ValidAudience = audience,
        //                    ValidateIssuerSigningKey = true,
        //                    IssuerSigningKey = signingKey,
        //                    ValidateLifetime = true,
        //                    ClockSkew = TimeSpan.Zero
        //                }, out _);
        //            }
        //            catch (SecurityTokenExpiredException)
        //            {
        //                // Токен просрочен — попробуем обновить
        //            }
        //            catch
        //            {
        //                // access_token невалиден — удалим
        //                context.Response.Cookies.Delete("access_token");
        //            }
        //        }

        //        if (principal == null && !string.IsNullOrEmpty(refreshToken))
        //        {
        //            try
        //            {
        //                var refreshPrincipal = handler.ValidateToken(refreshToken, new TokenValidationParameters
        //                {
        //                    ValidateIssuer = true,
        //                    ValidIssuer = issuer,
        //                    ValidateAudience = true,
        //                    ValidAudience = audience,
        //                    ValidateIssuerSigningKey = true,
        //                    IssuerSigningKey = signingKey,
        //                    ValidateLifetime = true,
        //                    ClockSkew = TimeSpan.Zero
        //                }, out _);

        //                var username = refreshPrincipal.Identity?.Name;
        //                var idClaim = refreshPrincipal.FindFirst(ClaimTypes.NameIdentifier);
        //                var userId = idClaim != null ? int.Parse(idClaim.Value) : (int?)null;
        //                var newAccessToken = GenerateToken(username, userId.Value, 1);

        //                context.Response.Cookies.Append("access_token", newAccessToken, new CookieOptions
        //                {
        //                    HttpOnly = true,
        //                    SameSite = SameSiteMode.Lax,
        //                    Expires = DateTime.UtcNow.AddMinutes(1)
        //                });

        //                // Добавим в заголовок, чтобы JwtBearer его увидел
        //                context.Request.Headers["Authorization"] = $"Bearer {newAccessToken}";
        //            }
        //            catch
        //            {
        //                context.Response.Cookies.Delete("refresh_token");
        //            }
        //        }

        //        // Устанавливаем principal в контексте, если он валиден
        //        if (principal != null)
        //        {
        //            context.User = principal;
        //        }

        //        await next();
        //    });

        //    // Другие middleware...
        //}

    }

    public class CreateUserRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
