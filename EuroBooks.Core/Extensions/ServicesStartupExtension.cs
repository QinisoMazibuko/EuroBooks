using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace EuroBooks.Infrastructure.Extensions
{
    public static class ServicesStartupExtension
    {

        public static IConfigurationRoot BuildAppSettings(this IConfiguration Configuration, IHostEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();
                //.AddJsonFile("appsettingscommon.json", optional: false, reloadOnChange: true)
                //.AddJsonFile($"appsettingscommon.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                //.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }

    }
}
