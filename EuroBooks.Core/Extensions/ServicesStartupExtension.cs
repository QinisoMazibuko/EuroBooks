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
               // .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            return builder.Build();
        }

    }
}
