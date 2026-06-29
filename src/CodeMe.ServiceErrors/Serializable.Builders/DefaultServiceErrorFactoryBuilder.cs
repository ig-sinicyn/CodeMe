namespace CodeMe.ServiceErrors.Serializable.Builders;

/// <summary>
/// Builder for creating a service error factory with configurable options.
/// </summary>
public sealed class DefaultServiceErrorFactoryBuilder

{
    private readonly ServiceErrorFactoryOptions _options;

    /// <summary>
    /// Initializes a new builder with the specified root error group.
    /// </summary>
    public DefaultServiceErrorFactoryBuilder(ErrorGroupUri rootErrorGroup)
    {
        _options = new ServiceErrorFactoryOptions
        {
            RootErrorGroup = rootErrorGroup
        };
    }

    /// <summary>
    /// Applies configuration to the builder options.
    /// </summary>
    public DefaultServiceErrorFactoryBuilder Configure(Action<ServiceErrorFactoryOptions> configure)
    {
        configure(_options);
        return this;
    }

    /// <summary>
    /// Builds and returns an instance of the service error factory.
    /// </summary>
    public IServiceErrorFactory Build() => new DefaultServiceErrorFactory(_options);
}