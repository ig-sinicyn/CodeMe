namespace CodeMe.ServiceErrors;

/// <summary>
/// Service error descriptor.
/// </summary>
/// <param name="Type">Service error type.</param>
/// <param name="StatusCode">Service error status.</param>
/// <param name="Transience">Service error transience.</param>
/// <param name="Severity">Service error severity.</param>
public sealed record ErrorDescriptor(
    ErrorUri Type,
    ErrorStatusCode StatusCode,
    ErrorTransience Transience = ErrorTransience.Normal,
    ErrorSeverity Severity = ErrorSeverity.ErrorResponse)
{
    /// <summary>
    /// Creates error descriptor.
    /// </summary>
    public static ErrorDescriptor Create(
        ErrorGroupUri group,
        string code,
        ErrorStatusCode statusCode,
        ErrorTransience transience = ErrorTransience.Normal,
        ErrorSeverity severity = ErrorSeverity.ErrorResponse) =>
        new(ErrorUri.Create(group, code), statusCode, transience, severity);

    /// <summary>
    /// Created descriptor for input validation error
    /// (400 Bad Request).
    /// </summary>
    public static ErrorDescriptor InvalidArgument(
        ErrorGroupUri group,
        string code,
        ErrorTransience transience = ErrorTransience.Normal,
        ErrorSeverity severity = ErrorSeverity.ErrorResponse) =>
        new(ErrorUri.Create(group, code), ErrorStatusCode.InvalidArgument, transience, severity);

    /// <summary>
    /// Created descriptor for requested entity not found error
    /// (404 Not Found).
    /// </summary>
    public static ErrorDescriptor NotFound(
        ErrorGroupUri group,
        string code,
        ErrorTransience transience = ErrorTransience.Normal,
        ErrorSeverity severity = ErrorSeverity.ErrorResponse) =>
        new(ErrorUri.Create(group, code), ErrorStatusCode.NotFound, transience, severity);

    /// <summary>
    /// Created descriptor for conflict when creating an entity error
    /// (409 Conflict).
    /// </summary>
    public static ErrorDescriptor AlreadyExists(
        ErrorGroupUri group,
        string code,
        ErrorTransience transience = ErrorTransience.Normal,
        ErrorSeverity severity = ErrorSeverity.ErrorResponse) =>
        new(ErrorUri.Create(group, code), ErrorStatusCode.AlreadyExists, transience, severity);

    /// <summary>
    /// Created descriptor for access denied error
    /// (403 Forbidden).
    /// </summary>
    public static ErrorDescriptor PermissionDenied(
        ErrorGroupUri group,
        string code,
        ErrorTransience transience = ErrorTransience.Normal,
        ErrorSeverity severity = ErrorSeverity.ErrorResponse) =>
        new(ErrorUri.Create(group, code), ErrorStatusCode.PermissionDenied, transience, severity);

    /// <summary>
    /// Created descriptor for invalid state error
    /// (412 Precondition Failed).
    /// </summary>
    public static ErrorDescriptor FailedPrecondition(
        ErrorGroupUri group,
        string code,
        ErrorTransience transience = ErrorTransience.Normal,
        ErrorSeverity severity = ErrorSeverity.ErrorResponse) =>
        new(ErrorUri.Create(group, code), ErrorStatusCode.FailedPrecondition, transience, severity);

    /// <summary>
    /// Created descriptor for internal error
    /// (500 Internal Server Error).
    /// </summary>
    public static ErrorDescriptor Internal(
        ErrorGroupUri group,
        string code,
        ErrorTransience transience = ErrorTransience.Normal,
        ErrorSeverity severity = ErrorSeverity.ErrorResponse) =>
        new(ErrorUri.Create(group, code), ErrorStatusCode.Internal, transience, severity);

    /// <summary>
    /// Error uri scheme.
    /// </summary>
    public string Scheme => Type.Group.Scheme;


    /// <summary>
    /// Application name.
    /// </summary>
    public string Application => Type.Group.Application;

    /// <summary>
    /// Error category.
    /// </summary>
    public string? Category => Type.Group.Category;

    /// <summary>
    /// Short error code.
    /// </summary>
    public string ShortCode => Type.Code;
}