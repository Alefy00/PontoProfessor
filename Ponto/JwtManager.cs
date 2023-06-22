using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Ponto.Security
{
    public static class JwtManager
    {
        private const string SecretKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjpbImlkYS5jZXViLmJyIiwicG9udG9wcm9mZXNzb3IiXSwibmFtZWlkIjoiU8OpcmdpbyBDb3p6ZXR0aSBCZXJ0b2xkaSBkZSBTb3V6YSIsImdpdmVuX25hbWUiOiJTw6lyZ2lvIENvenpldHRpIEJlcnRvbGRpIGRlIFNvdXphIiwicHJpbWFyeXNpZCI6IjMyODg2NDciLCJDcmVkZW50aWFsIjoie1wiSWRVc3VhcmlvXCI6MzI4ODY0NyxcIk5vbWVcIjpcIlPDqXJnaW8gQ296emV0dGkgQmVydG9sZGkgZGUgU291emFcIixcIkRSVFwiOlwiMDg0MzU0XCJ9IiwibmJmIjoxNjg3MDUxNzY1LCJleHAiOjE2ODcxMzgxNjUsImlhdCI6MTY4NzA1MTc2NSwiaXNzIjoiaHR0cHM6Ly9zZXJ2aWNvcy51bmljZXViLmJyLyJ9.P7IAwxcsdWftTzDGR_WXvxnK1jxPHyD1uYn0YAL7uNc\r\n"; // Coloque sua chave secreta aqui

        public static string GenerateToken(string matricula)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, matricula)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
