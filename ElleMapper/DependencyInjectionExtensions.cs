using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddDbContext<TContext>(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> optionsAction,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TContext : DbContext
        {
            services.Add(
                new ServiceDescriptor(
                    typeof(TContext),
                    sp =>
                    {
                        var optionsBuilder = new DbContextOptionsBuilder();
                        optionsAction(optionsBuilder);

                        var options = optionsBuilder._options;

                        return (TContext)ActivatorUtilities.CreateInstance(
                            sp,
                            typeof(TContext),
                            options
                        );
                    },
                    lifetime
                )
            );

            return services;
        }
    }
}
