using CodeMe.ServiceErrors.Serializable;
using CodeMe.ServiceErrors.Serializable.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace CodeMe.ServiceErrors.DependencyInjection;

internal sealed class ServiceErrorFactoryBuilder<TErrorFactory> : IServiceErrorFactoryBuilder
    where TErrorFactory : class, IServiceErrorFactory
{
    public ServiceErrorFactoryBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public ServiceErrorFactoryBuilder(
        IServiceCollection services,
        ErrorGroupUri rootErrorGroup,
        Type? implementationType)
    {
        Services = services;
        Init(rootErrorGroup, implementationType);
    }

    public IServiceCollection Services { get; }

    private void Init(ErrorGroupUri rootErrorGroup, Type? implementationType)
    {
        Services.AddOptions<ServiceErrorFactoryOptions<TErrorFactory>>()
            .Configure(
                opt =>
                {
                    opt.RootErrorGroup = rootErrorGroup;
                    opt.SchemeFillMode = ErrorDtoFillMode.IfUnknown;
                    opt.ApplicationFillMode = ErrorDtoFillMode.IfUnknown;
                    opt.CategoryFillMode = ErrorDtoFillMode.Always;
                });
        Services.TryAddSingleton<TErrorFactory>(
            x =>
            {
                var opt = x.GetRequiredService<IOptionsMonitor<ServiceErrorFactoryOptions<TErrorFactory>>>();
                var factory = ServiceErrorFactoryEmitter.CreateCallback<TErrorFactory>(implementationType);
                return factory(() => opt.CurrentValue);
            });
    }

    public IServiceErrorFactoryBuilder Configure(Action<ServiceErrorFactoryOptions> configure)
    {
        Services.AddOptions<ServiceErrorFactoryOptions<TErrorFactory>>().Configure(configure);
        return this;
    }

    public IServiceErrorFactoryBuilder Configure(Action<ServiceErrorFactoryOptions, IServiceProvider> configure)
    {
        Services.AddOptions<ServiceErrorFactoryOptions<TErrorFactory>>().Configure(configure);
        return this;
    }

    public IServiceCollection Build() => Services;
}