using Crayon.Api.Sdk;
using SingleUserApplication.Handlers;
using SingleUserApplication.Models;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace SingleUserApplication.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(
                scan => {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                    scan.With(new ControllerConvention());
                });

            var client = new CrayonApiClient(CrayonApiClient.ApiUrls.Demo);
            For<AppSettings>().Use(new AppSettings()).Singleton();
            For<CrayonApiClient>().Use(client).Singleton();
            For<TokenHandler>().Use<TokenHandler>().Singleton();
        }
    }
}