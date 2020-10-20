using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ePowerCCBApi
{
    public class ValidateTokenHandler : DelegatingHandler
    {
        private static bool TryRetriveToken(HttpRequestMessage request, out string token)
        {
            token = null;

            IEnumerable<string> authzHeaders;

            if (!request.Headers.TryGetValues("Authorization", out authzHeaders) || authzHeaders.Count() > 1)
                return false;

            var bearerToken = authzHeaders.ElementAt(0);

            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;

            return true;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpStatusCode statusCode;

            string token;

            if (!TryRetriveToken(request, out token))
            {
                statusCode = HttpStatusCode.Unauthorized;

                return base.SendAsync(request, cancellationToken);
            }

            try
            {
                const string Base64SecrectKey = "siO2ttKA2ONRF0LUHYFlG9OdTy1mYGwE0MBmBZE9EoOEAnE7rthdzeZRklpid+WKr5bgx6t3GLXURXu3JrIaYg==";
                var now = DateTime.UtcNow;
                var securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(Base64SecrectKey));

                SecurityToken securityToken;
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                TokenValidationParameters validationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    LifetimeValidator = this.LifetimeValidator,
                    IssuerSigningKey = securityKey

                };

                Thread.CurrentPrincipal = handler.ValidateToken(token, validationParameters, out securityToken);
                HttpContext.Current.User = handler.ValidateToken(token, validationParameters, out securityToken);

                return base.SendAsync(request, cancellationToken);
            }
            catch (SecurityTokenValidationException e)
            {

                statusCode = HttpStatusCode.Unauthorized;
            }
            catch (Exception ex)
            {
                statusCode = HttpStatusCode.InternalServerError;
            }

            return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(statusCode) { });
        }

        private bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (expires != null)
            {
                if (DateTime.UtcNow < expires)
                    return true;
            }
            return false;
        }
    }
}
