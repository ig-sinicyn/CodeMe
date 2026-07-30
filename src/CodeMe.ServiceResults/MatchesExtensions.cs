using CodeMe.ServiceErrors;

namespace CodeMe.ServiceResults;

/// <summary>
/// Provides extension methods for matching service result errors.
/// </summary>
public static class MatchesExtensions
{
    /// <summary>
    /// Determines whether the service result contains an error with the specified status code.
    /// </summary>
    public static bool Matches(this ServiceResult result, ErrorStatusCode statusCode) =>
        result.Error is { } error && error.MatchesAny(statusCode);

    /// <summary>
    /// Determines whether the service result contains an error with the specified status code.
    /// </summary>
    public static bool Matches<T>(this ServiceResult<T> result, ErrorStatusCode statusCode) =>
        result.Error is { } error && error.MatchesAny(statusCode);

    /// <summary>
    /// Determines whether the service result contains an error with any of the specified status codes.
    /// </summary>
    public static bool MatchesAny(this ServiceResult result, params ReadOnlySpan<ErrorStatusCode> statusCodes) =>
        result.Error is { } error && error.MatchesAny(statusCodes);

    /// <summary>
    /// Determines whether the service result contains an error with any of the specified status codes.
    /// </summary>
    public static bool MatchesAny<T>(this ServiceResult<T> result, params ReadOnlySpan<ErrorStatusCode> statusCodes) =>
        result.Error is { } error && error.MatchesAny(statusCodes);

    /// <summary>
    /// Determines whether the service result contains an error with any of the status codes in the sequence.
    /// </summary>
    public static bool MatchesAny(this ServiceResult result, IEnumerable<ErrorStatusCode> statusCodes) =>
        result.Error is { } error && error.MatchesAny(statusCodes);

    /// <summary>
    /// Determines whether the service result contains an error with any of the status codes in the sequence.
    /// </summary>
    public static bool MatchesAny<T>(this ServiceResult<T> result, IEnumerable<ErrorStatusCode> statusCodes) =>
        result.Error is { } error && error.MatchesAny(statusCodes);

    /// <summary>
    /// Determines whether the service result contains an error in the specified error group.
    /// </summary>
    public static bool Matches(this ServiceResult result, ErrorGroupUri errorGroup) =>
        result.Error is { } error && error.Matches(errorGroup);

    /// <summary>
    /// Determines whether the service result contains an error in the specified error group.
    /// </summary>
    public static bool Matches<T>(this ServiceResult<T> result, ErrorGroupUri errorGroup) =>
        result.Error is { } error && error.Matches(errorGroup);

    /// <summary>
    /// Determines whether the service result contains an error in any of the specified error groups.
    /// </summary>
    public static bool MatchesAny(this ServiceResult result, params ReadOnlySpan<ErrorGroupUri> errorGroups) =>
        result.Error is { } error && error.MatchesAny(errorGroups);

    /// <summary>
    /// Determines whether the service result contains an error in any of the specified error groups.
    /// </summary>
    public static bool MatchesAny<T>(this ServiceResult<T> result, params ReadOnlySpan<ErrorGroupUri> errorGroups) =>
        result.Error is { } error && error.MatchesAny(errorGroups);

    /// <summary>
    /// Determines whether the service result contains an error in any of the error groups in the sequence.
    /// </summary>
    public static bool MatchesAny(this ServiceResult result, IEnumerable<ErrorGroupUri> errorGroups) =>
        result.Error is { } error && error.MatchesAny(errorGroups);

    /// <summary>
    /// Determines whether the service result contains an error in any of the error groups in the sequence.
    /// </summary>
    public static bool MatchesAny<T>(this ServiceResult<T> result, IEnumerable<ErrorGroupUri> errorGroups) =>
        result.Error is { } error && error.MatchesAny(errorGroups);

    /// <summary>
    /// Determines whether the service result contains an error with the specified error type.
    /// </summary>
    public static bool Matches(this ServiceResult result, ErrorUri type) =>
        result.Error is { } error && error.Matches(type);

    /// <summary>
    /// Determines whether the service result contains an error with the specified error type.
    /// </summary>
    public static bool Matches<T>(this ServiceResult<T> result, ErrorUri type) =>
        result.Error is { } error && error.Matches(type);

    /// <summary>
    /// Determines whether the service result contains an error with any of the specified error types.
    /// </summary>
    public static bool MatchesAny(this ServiceResult result, params ReadOnlySpan<ErrorUri> types) =>
        result.Error is { } error && error.MatchesAny(types);

    /// <summary>
    /// Determines whether the service result contains an error with any of the specified error types.
    /// </summary>
    public static bool MatchesAny<T>(this ServiceResult<T> result, params ReadOnlySpan<ErrorUri> types) =>
        result.Error is { } error && error.MatchesAny(types);

    /// <summary>
    /// Determines whether the service result contains an error with any of the error types in the sequence.
    /// </summary>
    public static bool MatchesAny(this ServiceResult result, IEnumerable<ErrorUri> types) =>
        result.Error is { } error && error.MatchesAny(types);

    /// <summary>
    /// Determines whether the service result contains an error with any of the error types in the sequence.
    /// </summary>
    public static bool MatchesAny<T>(this ServiceResult<T> result, IEnumerable<ErrorUri> types) =>
        result.Error is { } error && error.MatchesAny(types);

    /// <summary>
    /// Determines whether the service result contains an error that matches the specified error descriptor.
    /// </summary>
    public static bool Matches(this ServiceResult result, ErrorDescriptor errorDescriptor) =>
        result.Error is { } error && error.Matches(errorDescriptor);

    /// <summary>
    /// Determines whether the service result contains an error that matches the specified error descriptor.
    /// </summary>
    public static bool Matches<T>(this ServiceResult<T> result, ErrorDescriptor errorDescriptor) =>
        result.Error is { } error && error.Matches(errorDescriptor);

    /// <summary>
    /// Determines whether the service result contains an error that matches any of the specified error descriptors.
    /// </summary>
    public static bool MatchesAny(this ServiceResult result, params ReadOnlySpan<ErrorDescriptor> descriptors) =>
        result.Error is { } error && error.MatchesAny(descriptors);

    /// <summary>
    /// Determines whether the service result contains an error that matches any of the specified error descriptors.
    /// </summary>
    public static bool MatchesAny<T>(this ServiceResult<T> result, params ReadOnlySpan<ErrorDescriptor> descriptors) =>
        result.Error is { } error && error.MatchesAny(descriptors);

    /// <summary>
    /// Determines whether the service result contains an error that matches any of the error descriptors in the sequence.
    /// </summary>
    public static bool MatchesAny(this ServiceResult result, IEnumerable<ErrorDescriptor> descriptors) =>
        result.Error is { } error && error.MatchesAny(descriptors);

    /// <summary>
    /// Determines whether the service result contains an error that matches any of the error descriptors in the sequence.
    /// </summary>
    public static bool MatchesAny<T>(this ServiceResult<T> result, IEnumerable<ErrorDescriptor> descriptors) =>
        result.Error is { } error && error.MatchesAny(descriptors);
}
