using System.ComponentModel.DataAnnotations;

namespace CodeMe.ServiceErrors.Serializable.Builders;

using ExceptionFactory = Func<ServiceError, IServiceException>;

/// <summary>
/// DI-compatible configuration options for service error factory.
/// </summary>
public class ServiceErrorFactoryOptions : IServiceErrorFactoryOptions
{
    /// <inheritdoc/>
    [Required]
    public required ErrorGroupUri RootErrorGroup { get; set; }

    /// <inheritdoc/>
    public ErrorDtoFillMode SchemeFillMode { get; set; }

    /// <inheritdoc/>
    public ErrorDtoFillMode ApplicationFillMode { get; set; }

    /// <inheritdoc/>
    public ErrorDtoFillMode CategoryFillMode { get; set; }

    /// <summary>
    /// Short error code to descriptor mapping for <see cref="ErrorUri.Code"/>.
    /// </summary>
    public Dictionary<string, ErrorDescriptor> ErrorCodeMapping { get; } =
        new(StringComparer.FromComparison(ErrorGroupUri.UriPartComparison));

    /// <inheritdoc/>
    public ExceptionFactory? DefaultErrorFactory { get; set; }

    /// <summary>
    /// Exception factories for well-known error types.
    /// </summary>
    public Dictionary<ErrorUri, ExceptionFactory> ErrorFactories { get; } = [];

    /// <summary>
    /// Exception factories for well-known error groups.
    /// </summary>
    public Dictionary<ErrorGroupUri, ExceptionFactory> ErrorGroupFactories { get; } = [];

    /// <inheritdoc/>
    IReadOnlyDictionary<string, ErrorDescriptor> IServiceErrorFactoryOptions.ErrorCodeMapping =>
        ErrorCodeMapping;

    /// <inheritdoc/>
    IReadOnlyDictionary<ErrorUri, ExceptionFactory> IServiceErrorFactoryOptions.ErrorFactories =>
        ErrorFactories;

    /// <inheritdoc/>
    IReadOnlyDictionary<ErrorGroupUri, ExceptionFactory> IServiceErrorFactoryOptions.ErrorGroupFactories =>
        ErrorGroupFactories;
}