using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApplication1.DataBase_and_more
{
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer"; 
        public const string AUDIENCE = "MyAuthClient"; 
        const string KEY = "mysupersecret_secretkey!123";  
        public const int LIFETIME = 1;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }

        public static void SetJwtCookie(HttpContext context, Guid userId)
        {
            var token = GenerateJwtToken(userId); // Ваш метод генерации JWT

            context.Response.Cookies.Append("jwt_token", token, new CookieOptions
            {
                HttpOnly = true, // Защита от XSS
                Secure = true,    // Только HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.Now.AddDays(30) // Срок действия
            });
        }

        public static string GenerateJwtToken(Guid userId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetSymmetricSecurityKey().ToString()));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: ISSUER,
                audience: AUDIENCE,
                claims: new[] { new Claim("userId", userId.ToString()) },
                expires: DateTime.Now.AddYears(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static bool ValidateToken(string token, out Guid userId)
        {
            userId = Guid.Empty;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = GetSymmetricSecurityKey();

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key.ToString())),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = ISSUER,
                    ValidAudience = AUDIENCE,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "userId").Value);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
