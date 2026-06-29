namespace CodeMe.ServiceErrors;

/// <summary>
/// Service error severity.
/// </summary>
public enum ErrorSeverity
{
    /// <summary>
    /// Normal error that should be returned to the user.
    /// Examples: validation error, record not found, etc.
    /// </summary>
    ErrorResponse,

    /// <summary>
    /// Internal non-critical error.
    /// Examples: dto conversion error, service call returned unexpected response.
    /// </summary>
    InternalError,

    /// <summary>
    /// Unknown and unexpected error that should be caught in logs and converted to one of the two categories above.
    /// Examples: SQL query errors, internal assert violation.
    /// </summary>
    Unexpected,

    /// <summary>
    /// Serious failure, service is inoperable.
    /// Examples: corrupted internal state, missing database, etc.
    /// </summary>
    Fatal
}