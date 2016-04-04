using Crayon.Api.Sdk;
using MultiUserApplication.Models;

namespace MultiUserApplication.DependencyResolution
{
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;

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

            For<AppSettings>().Use(new AppSettings()).Singleton();
            For<CrayonApiClient>().Use(new CrayonApiClient(CrayonApiClient.ApiUrls.Demo)).Singleton();
        }
    }
}