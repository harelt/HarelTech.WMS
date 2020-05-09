using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HarelTech.WMS.Common.Models;
using System.Threading.Tasks;

namespace HarelTech.WMS.Api.Models
{
    public interface IUserService
    {
        Task<AuthUser> Authenticate(string username, string password);
        
    }
    public class UserService: IUserService
    {
        private readonly IConfiguration _configuration;
        private List<AuthUser> _users = new List<AuthUser>
        {
            new AuthUser { Id = 1, FirstName = "Harel", LastName = "Tech", Username = "HarelTech", Password = "harel1@tech2!" }
        };

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<AuthUser> Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:JwtSecret").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);


            return await Task.FromResult(user.WithoutPassword());
        }

    }
}
