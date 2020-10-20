using Microsoft.IdentityModel.Tokens;
using StandAloneJWTTokenAPI.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;

namespace StandAloneJWTTokenAPI.Controllers
{
    public class AuthenticateController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Authenticate([FromBody] UserRequest user)
        {
            try
            {
                UserRequest userRequest = new UserRequest { };
                userRequest.Username = user.Username.ToLower();
                userRequest.Password = user.Password;

                bool isUsernamePasswordValid = false;

                if (user != null)
                    isUsernamePasswordValid = userRequest.Password == "admin" ? true : false;

                if (isUsernamePasswordValid)
                {
                    string token = createToken(user.Username);

                    return Ok<string>(token);
                }
                else
                {
                    return BadRequest("Usuario invalido");
                }

            }
            catch (Exception e)
            {

                return BadRequest(e.ToString());
            }
            

        }

        private string createToken(string username)
        {
            var minutes = 30;
            DateTime issueAt = DateTime.UtcNow;
            DateTime expires = DateTime.UtcNow.AddMinutes(minutes);

            var tokenHandler = new JwtSecurityTokenHandler();

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            });

            const string Base64SecrectKey = "siO2ttKA2ONRF0LUHYFlG9OdTy1mYGwE0MBmBZE9EoOEAnE7rthdzeZRklpid+WKr5bgx6t3GLXURXu3JrIaYg==";
            var now = DateTime.UtcNow;
            var securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(Base64SecrectKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var token = (JwtSecurityToken)tokenHandler.CreateJwtSecurityToken
                (
                    issuer: "https://localhost:44382",
                    audience: "https://localhost:44382",
                    subject: claimsIdentity,
                    notBefore: issueAt,
                    expires: expires,
                    signingCredentials: signingCredentials
                );

            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
    }
}
