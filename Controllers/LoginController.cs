using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace sample_graphql_api.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        public LoginController(IConfiguration Configuration, ILogger<LoginController> logger)
        {
            _configuration = Configuration;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Login(string userName, string password)
        {
            if (IsValidUserNameAndPassword(userName, password))
            {
                return new ObjectResult(new { Token = GenerateToken(userName) });
            }
            return Unauthorized();
        }

        private bool IsValidUserNameAndPassword(string userName, string password)
        {
            //Bu kısımda database'e gidip bu kullanıcı adı ve şifre varmı diye kontrol edilebilir yada bir business işletilebilir.
            //Ben şimdilik bu kısmı simule ediyorum.
            return true;
        }

        //Token oluşturmak için kullandığımız method.
        private string GenerateToken(string userName)
        {
            var parameters = new Claim[]{
                new Claim(JwtRegisteredClaimNames.UniqueName,userName)
            };

            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes((string)Convert.ChangeType(_configuration["JwtTokenConfig:IssuerSigningKey"], typeof(string)))); // Startupda yazdığımız olması gerek.
            var tokenLifeTime = (double)Convert.ChangeType(_configuration["JwtTokenConfig:TokenLifeTime"], typeof(double));
            // _logger.Log(LogLevel.Debug, message: lifeTime.ToString());
            // _logger.LogDebug(message: DateTime.UtcNow.AddMinutes(lifeTime).ToString("dd.MM.yyyy hh:mm"));
            var token = new JwtSecurityToken(
                issuer: (string)Convert.ChangeType(_configuration["JwtTokenConfig:ValidIssuer"], typeof(string)),
                audience: (string)Convert.ChangeType(_configuration["JwtTokenConfig:ValidAudience"], typeof(string)),
                claims: parameters,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(tokenLifeTime), //Jwt Token Utc olarak ayarlıyo 
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
                );
            // _logger.Log(LogLevel.Critical, token.ValidTo.ToString("dd.MM.yyyy hh:mm"));
            // _logger.Log(LogLevel.Critical, token.ValidFrom.ToString("dd.MM.yyyy hh:mm"));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}