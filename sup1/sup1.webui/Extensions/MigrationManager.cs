using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using sup1.data.Concrete.EfCore;
using sup1.webui.Identity;

namespace sup1.webui.Extensions
{
    public static class MigrationManager
    {
        public static IHost MigrateDatabase(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                using (var accountContext = scope.ServiceProvider.GetRequiredService<AccountContext>())
                {
                    try
                    {
                        accountContext.Database.Migrate();
                    }
                    catch (System.Exception)
                    {
                        // Logging
                        throw;
                    }
                }

                using (var supContext = scope.ServiceProvider.GetRequiredService<SupContext>())
                {
                    try
                    {
                        supContext.Database.Migrate();
                    }
                    catch (System.Exception)
                    {
                        // Logging
                        throw;
                    }
                }
            }

            return host;
        }
    }
}