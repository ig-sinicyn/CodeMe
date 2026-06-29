namespace CodeMe.ServiceErrors;

/// <summary>
/// Service error status code (RFC 9457-compatible, GRPC-compatible status code).
/// </summary>
/// <remarks>
/// Based on GRPC errors (grpc_status_code from 'grpc/status'),
/// as they have fewer states than http codes.
/// https://grpc.io/docs/guides/status-codes/.
/// </remarks>
public enum ErrorStatusCode
{
    /// <summary>
    /// Operation cancelled (499 Client Closed Request).
    /// </summary>
    Cancelled = 1,

    /// <summary>
    /// Unknown error (500 Internal Server Error).
    /// </summary>
    Unknown = 2,

    /// <summary>
    /// Input validation error (400 Bad Request).
    /// </summary>
    InvalidArgument = 3,

    /// <summary>
    /// Request processing time exceeded (408 Request Timeout).
    /// </summary>
    DeadlineExceeded = 4,

    /// <summary>
    /// Requested entity not found (404 Not Found).
    /// </summary>
    NotFound = 5,

    /// <summary>
    /// Conflict when creating an entity (409 Conflict).
    /// </summary>
    AlreadyExists = 6,

    /// <summary>
    /// Access denied (403 Forbidden).
    /// </summary>
    PermissionDenied = 7,

    /// <summary>
    /// Resource exhausted (429 Too Many Requests).
    /// </summary>
    ResourceExhausted = 8,

    /// <summary>
    /// Invalid state (412 Precondition Failed).
    /// </summary>
    FailedPrecondition = 9,

    /// <summary>
    /// Request aborted (444 No Response).
    /// </summary>
    Aborted = 10,

    /// <summary>
    /// Invalid range of values (416 Range Not Satisfiable).
    /// </summary>
    OutOfRange = 11,

    /// <summary>
    /// Logic not implemented (501 Not Implemented).
    /// </summary>
    Unimplemented = 12,

    /// <summary>
    /// Internal error (500 Internal Server Error).
    /// </summary>
    Internal = 13,

    /// <summary>
    /// Service unavailable (503 Service Unavailable).
    /// </summary>
    Unavailable = 14,

    /// <summary>
    /// Data loss (410 Gone).
    /// </summary>
    DataLoss = 15,

    /// <summary>
    /// Authorization required (401 Unauthorized).
    /// </summary>
    Unauthenticated = 16
}