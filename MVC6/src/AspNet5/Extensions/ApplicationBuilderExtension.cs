using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace MVC6.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static IApplicationBuilder UseCrayonAuthentication(
            this IApplicationBuilder app,
            string clientId,
            string authority,
            string siteUrl,
            PathString callbackPath,
            bool requireHttps)
        {
            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions {
                Authority = authority,
                PostLogoutRedirectUri = siteUrl,
                CallbackPath = callbackPath,
                ClientId = clientId,
                Scope = { "CustomerApi" },
                SignInScheme = "Cookies",
                ResponseType = "id_token token",
                ResponseMode = "form_post",
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = new TokenValidationParameters {
                    ValidateLifetime = true                    
                },
                GetClaimsFromUserInfoEndpoint = true,
                ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                     authority + ".well-known/openid-configuration",
                    new OpenIdConnectConfigurationRetriever(),
                    new HttpDocumentRetriever { RequireHttps = requireHttps }
                ),
                Events = new OpenIdConnectEvents {
                    OnTokenValidated = n => {
                        var incoming = n.Ticket.Principal;
                        var id = (ClaimsIdentity)incoming.Identity;
                        id.AddClaim(new Claim("token", n.ProtocolMessage.AccessToken));                     
                        n.HandleResponse();
                        return Task.FromResult(0);
                    }
                }
            });

            return app;
        }
    }
}