using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Xml.Linq;

namespace DemoAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public static JwtSecurityTokenHandler JwtTokenHandler = new JwtSecurityTokenHandler();
        public static SymmetricSecurityKey SecurityKey = new SymmetricSecurityKey(Guid.NewGuid().ToByteArray());

        [HttpPost("/CreateToken")]
        public string GetToken()
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "login") };
            var credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                "ExampleServer",
                "ExampleClients",
                claims,
                expires: DateTime.Now.AddSeconds(60),
                signingCredentials: credentials);
            return JwtTokenHandler.WriteToken(token);
        }

        [HttpPost("/SignIn")]
        public async Task Login(string login, string password)
        {

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, login)
                };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await this.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties()
                    {
                        ExpiresUtc = DateTime.Now.AddSeconds(60)
                    });
        }

        [HttpPost("/SignOut")]
        public async Task SignOut()
        {
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

    }
}
