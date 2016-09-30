using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace MVC6
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel();

            if (args.Length > 0)
            {
                host = host.UseEnvironment(args[0]);
            }

            host
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}