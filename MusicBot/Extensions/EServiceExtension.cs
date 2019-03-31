using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MusicBot.Extensions
{
        public static class EServiceExtention
        {
            public static IServiceCollection AutoAddServices(this IServiceCollection services)
            {
                foreach (var type in Assembly.GetExecutingAssembly().GetTypes()
                    .Where(x => typeof(IServiceExtension).IsAssignableFrom(x) && !x.IsInterface))
                {
                    services.AddSingleton(type);
                }
                return services;
            }

            public static async Task InitializeServicesAsync(this IServiceProvider services)
            {
                foreach (var type in Assembly.GetExecutingAssembly().GetTypes()
                    .Where(x => typeof(IServiceExtension).IsAssignableFrom(x) && !x.IsInterface))
                {
                    await ((IServiceExtension)services.GetRequiredService(type)).InitializeAsync();
                }
            }
        }
}
