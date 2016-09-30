using MVC6.Extensions;
using Crayon.Api.Sdk;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace MVC6
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton(x => new CrayonApiClient(CrayonApiClient.ApiUrls.Demo));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            const string currentSiteUrl = "http://localhost:3181/";

            app.UseBrowserLink();
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            
            app.UseCookieAuthentication(new CookieAuthenticationOptions {
                AuthenticationScheme = "Cookies",
                AutomaticAuthenticate = true
            });

            app.UseCrayonAuthentication(
                Configuration["CrayonClientId"],
                CrayonApiClient.AuthorityUrls.Demo,
                currentSiteUrl,
                PathString.FromUriComponent("/callback/"),
                true);

            app.UseDeveloperExceptionPage();
            app.UseMvcWithDefaultRoute();
        }
    }
}