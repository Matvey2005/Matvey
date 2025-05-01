using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Курсач_1.Models;

namespace Курсач_1.Repositories
{
    public class UserRepositories
    {
        private readonly ApplicationDbContext _dbContext;
        static string secretKey = "super_super_secret_key_with_256_bits_!!!"; // 256+ бит
        static string issuer = "MyApp";
        static string audience = "MyUsers";
        static SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        public UserRepositories(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Add(string username, string password)
        {
            // Создайте экземпляр PasswordHasher
            var passwordHasher = new PasswordHasher<User>();
            if (_dbContext.Users.Select(x => x.UserName).ToList().Contains(username)) return false;
            // Хэшируйте пароль
            var hashedPassword = passwordHasher.HashPassword(null, password);

            var user = new User
            {
                UserName = username,
                // Сохраните только хэшированный пароль
                Password = hashedPassword
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }


        public async Task<User> GetUser(string Name, string Password)
        {
            var passwordHasher = new PasswordHasher<User>();

            // Хэшируйте пароль
            //var hashedPassword = passwordHasher.HashPassword(null, Password);

            User user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == Name);
            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, Password);
            //bool a = user.Password == hashedPassword;
            return verificationResult == PasswordVerificationResult.Success ? user : null;
        }
        

        
    }

    
}
