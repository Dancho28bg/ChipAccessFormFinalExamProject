using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ChipAccess.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ChipAccess.Api.Services
{
    public interface IAuthService
    {
        Task<AuthResult?> LoginAsync(string bamId);
    }

    public class AuthService : IAuthService
    {
        private readonly IEmployeeService _employeeService;
        private readonly IConfiguration _config;

        public AuthService(IEmployeeService employeeService, IConfiguration config)
        {
            _employeeService = employeeService;
            _config = config;
        }

        public async Task<AuthResult?> LoginAsync(string bamId)
        {
            var user = await _employeeService.GetByBamIdAsync(bamId);

            if (user == null || !user.IsActive)
                return null;

            var token = GenerateJwtToken(user);

            return new AuthResult
            {
                BamId = user.BamId,
                Email = user.Email,
                Role = user.Role,
                Token = token
            };
        }

        private string GenerateJwtToken(Employee user)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.BamId),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, user.Role ?? "User")
            };

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class AuthResult
    {
        public string BamId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
