namespace CodeMe.ServiceErrors.Serializable;

/// <summary>
/// Immutable <see cref="IServiceErrorFactoryOptions"/> implementation.
/// Used mostly for testing and as a default options instance for <see cref="DefaultServiceErrorFactory"/>.
/// </summary>
public sealed record DefaultServiceErrorFactoryOptions(
    ErrorGroupUri RootErrorGroup,
    ErrorDtoFillMode SchemeFillMode,
    ErrorDtoFillMode ApplicationFillMode,
    ErrorDtoFillMode CategoryFillMode,
    IReadOnlyDictionary<string, ErrorDescriptor> ErrorCodeMapping,
    Func<ServiceError, IServiceException>? DefaultErrorFactory,
    IReadOnlyDictionary<ErrorUri, Func<ServiceError, IServiceException>> ErrorFactories,
    IReadOnlyDictionary<ErrorGroupUri, Func<ServiceError, IServiceException>> ErrorGroupFactories)
    : IServiceErrorFactoryOptions
{
    /// <summary>
    /// Immutable <see cref="IServiceErrorFactoryOptions"/> implementation.
    /// Used mostly for testing and as a default options instance for <see cref="DefaultServiceErrorFactory"/>.
    /// </summary>
    public static DefaultServiceErrorFactoryOptions Create(ErrorGroupUri rootErrorGroup) =>
        new(
            rootErrorGroup,
            ErrorDtoFillMode.IfUnknown,
            ErrorDtoFillMode.IfUnknown,
            ErrorDtoFillMode.IfUnknown,
            new Dictionary<string, ErrorDescriptor>(),
            null,
            new Dictionary<ErrorUri, Func<ServiceError, IServiceException>>(),
            new Dictionary<ErrorGroupUri, Func<ServiceError, IServiceException>>());

    /// <summary>
    /// Immutable <see cref="IServiceErrorFactoryOptions"/> implementation.
    /// Used mostly for testing and as a default options instance for <see cref="DefaultServiceErrorFactory"/>.
    /// </summary>
    public static DefaultServiceErrorFactoryOptions Create(IServiceErrorFactoryOptions options) =>
        new(
            RootErrorGroup: options.RootErrorGroup,
            SchemeFillMode: options.SchemeFillMode,
            ApplicationFillMode: options.ApplicationFillMode,
            CategoryFillMode: options.CategoryFillMode,
            ErrorCodeMapping: options.ErrorCodeMapping,
            DefaultErrorFactory: options.DefaultErrorFactory,
            ErrorFactories: options.ErrorFactories,
            ErrorGroupFactories: options.ErrorGroupFactories);
}