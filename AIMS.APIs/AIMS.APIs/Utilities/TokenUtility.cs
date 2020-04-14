using AIMS.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace AIMS.APIs.Utilities
{
    public class TokenUtility
    {
        public string GenerateToken(TokenModel model)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, model.Id),
                new Claim(ClaimTypes.Email, model.Email),
                new Claim(ClaimTypes.Role, model.UserType.ToString()),
                new Claim(ClaimTypes.Country, model.OrganizationId),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(model.JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: model.JwtIssuer,
                audience: model.JwtAudience,
                claims: claims,
                expires: DateTime.Now.AddHours(12),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GeneratePasswordResetToken(PasswordTokenModel model)
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            string token = Convert.ToBase64String((time.Concat(key).ToArray()));
            return token;
        }

        public DateTime GetDecodedResetToken(string token)
        {
            DateTime tokenTime = DateTime.Now;
            try
            {
                byte[] data = Convert.FromBase64String(token);
                PasswordTokenModel model = new PasswordTokenModel();
                tokenTime = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
                return tokenTime;
            }
            catch(Exception ex)
            {
                string message = ex.Message;
            }
            return tokenTime;
        }
    }
}
