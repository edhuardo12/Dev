using ePowerCCBApi.ePowerBridge;
using ePowerCCBApi.Models;
using Microsoft.IdentityModel.Tokens;
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


namespace ePowerCCBApi.Controllers
{
    public class ePowerAuthenticateController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> Authenticate([FromBody] UserRequest user)
        {
            ePowerBridge.ServicioEpowerClient _servicioEpower = new ServicioEpowerClient();

            IHttpActionResult response;

            try
            {
                if (user.mode == 1)
                {
                    var info = await _servicioEpower.LoginePowerAsync(user.Username, user.Password);

                    if (info.Resultado == false)
                    {

                        response = BadRequest(info.Mensaje);

                        return response;
                    }

                    else
                    {
                        Configuration objConfig = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                        AppSettingsSection objAppsettings = (AppSettingsSection)objConfig.GetSection("appSettings");
                        //Edit
                        if (objAppsettings != null)
                        {
                            objAppsettings.Settings["currentUserName"].Value = user.Username;
                            objAppsettings.Settings["currentUserPassword"].Value = user.Password;
                            objConfig.Save();
                        }


                        string token = createToken(user.Username);

                        return Ok<string>(token);
                    }

                }
                else
                {
                    var info = await _servicioEpower.LoginePowerTestAsync(user.Username, user.Password);
                    
                    if (info.Resultado == false)
                    {

                        response = BadRequest(info.Mensaje);

                        return response;
                    }
                    else
                    {

                        string token = createToken(user.Username);

                        return Ok<string>(token);
                    }
                }
            }
            
            catch (Exception ex)
            {

                return BadRequest(ex.ToString());
            }

        }

        private string createToken(string username)
        {
            var minutes = Convert.ToInt32(ConfigurationManager.AppSettings["LifeTimeTokenMinutes"].ToString());
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
