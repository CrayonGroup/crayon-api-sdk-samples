using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Authentication.OpenIdConnect;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

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
            app.UseOpenIdConnectAuthentication(options => {
                options.Authority = authority;
                options.PostLogoutRedirectUri = siteUrl;
                options.CallbackPath = callbackPath;
                options.ClientId = clientId;
                options.Scope.Add("CustomerApi");
                options.SignInScheme = "Cookies";
                options.ResponseType = "id_token token";
                options.ResponseMode = "form_post";
                options.AutomaticAuthenticate = true;
                options.AutomaticChallenge = true;
                options.TokenValidationParameters.ValidateLifetime = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    options.Authority + ".well-known/openid-configuration",
                    new OpenIdConnectConfigurationRetriever(),
                    new HttpDocumentRetriever { RequireHttps = requireHttps }
                    );
                options.Events = new OpenIdConnectEvents {
                    OnAuthenticationValidated = n => {
                        var incoming = n.AuthenticationTicket.Principal;
                        var id = new ClaimsIdentity("application", "name", "role");
                        id.AddClaimsFromIdentity(incoming, new List<string> {
                            "name",
                            "email",
                            "email_verified",
                            ClaimTypes.Email,
                            ClaimTypes.NameIdentifier,
                            ClaimTypes.Role,
                            ClaimTypes.Name,
                            ClaimTypes.GivenName,
                            ClaimTypes.Surname
                        });

                        id.AddClaim(new Claim("token", n.ProtocolMessage.AccessToken));
                        n.AuthenticationTicket = new AuthenticationTicket(
                            new ClaimsPrincipal(id),
                            n.AuthenticationTicket.Properties,
                            n.AuthenticationTicket.AuthenticationScheme);

                        n.HandleResponse();
                        return Task.FromResult(0);
                    }
                };
            });

            return app;
        }
    }
}