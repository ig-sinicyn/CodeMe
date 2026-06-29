using CodeMe.ServiceErrors.Serializable.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace CodeMe.ServiceErrors.DependencyInjection;

/// <summary>
/// Contract for building and configuring a service error factory in the DI container.
/// </summary>
public interface IServiceErrorFactoryBuilder
{
    /// <summary>
    /// Service collection to which the service error factory is being added.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Adds configuration for the service error factory options.
    /// </summary>
    public IServiceErrorFactoryBuilder Configure(Action<ServiceErrorFactoryOptions> configure);

    /// <summary>
    /// Adds configuration for the service error factory options with access to the service provider.
    /// </summary>
    public IServiceErrorFactoryBuilder Configure(Action<ServiceErrorFactoryOptions, IServiceProvider> configure);

    /// <summary>
    /// Completes the configuration and returns the service collection for further chaining.
    /// </summary>
    public IServiceCollection Build();
}