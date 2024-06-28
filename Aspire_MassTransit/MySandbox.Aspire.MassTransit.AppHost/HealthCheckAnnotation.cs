using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MySandbox.Aspire.MassTransit.AppHost
{
    /// <summary>
    /// An annotation that associates a health check factory with a resource
    /// </summary>
    /// <param name="healthCheckFactory">A function that creates the health check</param>
    /// <remarks>See <see href="https://github.com/davidfowl/WaitForDependenciesAspire/tree/main"/></remarks>
    public class HealthCheckAnnotation(Func<IResource, CancellationToken, Task<IHealthCheck?>> healthCheckFactory) : IResourceAnnotation
    {
        public Func<IResource, CancellationToken, Task<IHealthCheck?>> HealthCheckFactory { get; } = healthCheckFactory;

        public static HealthCheckAnnotation Create(Func<string, IHealthCheck> connectionStringFactory)
        {
            return new(async (resource, token) =>
            {
                if (resource is not IResourceWithConnectionString c)
                {
                    return null;
                }

                if (await c.GetConnectionStringAsync(token) is not string cs)
                {
                    return null;
                }

                return connectionStringFactory(cs);
            });
        }
    }
}
