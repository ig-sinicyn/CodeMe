using CodeMe.ServiceErrors.Serializable;
using CodeMe.ServiceErrors.Serializable.Builders;

namespace CodeMe.ServiceErrors.DependencyInjection;

/// <summary>
/// DI-compatible configuration options for typed error factory.
/// </summary>
public class ServiceErrorFactoryOptions<TErrorFactory> : ServiceErrorFactoryOptions
    where TErrorFactory : class, IServiceErrorFactory
{
}