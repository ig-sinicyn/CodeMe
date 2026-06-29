namespace CodeMe.ServiceErrors;

/// <summary>
/// Provides extension methods for matching error descriptors, service errors, and service exceptions.
/// </summary>
public static class MatchesExtensions
{
    /// <summary>
    /// Determines whether the error descriptor has the specified status code.
    /// </summary>
    public static bool Matches(this ErrorDescriptor error, ErrorStatusCode statusCode) =>
        error.StatusCode == statusCode;

    /// <summary>
    /// Determines whether the error descriptor has any of the specified status codes.
    /// </summary>
    public static bool MatchesAny(this ErrorDescriptor error, params ReadOnlySpan<ErrorStatusCode> statusCodes)
    {
        foreach (var statusCode in statusCodes)
        {
            if (error.Matches(statusCode))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether the error descriptor has any of the status codes in the sequence.
    /// </summary>
    public static bool MatchesAny(this ErrorDescriptor error, IEnumerable<ErrorStatusCode> statusCodes)
    {
        foreach (var statusCode in statusCodes)
        {
            if (error.Matches(statusCode))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether the error descriptor is in the specified error group.
    /// </summary>
    public static bool Matches(this ErrorDescriptor error, ErrorGroupUri errorGroup) =>
        errorGroup.Contains(error.Type);

    /// <summary>
    /// Determines whether the error descriptor is in any of the specified error groups.
    /// </summary>
    public static bool MatchesAny(this ErrorDescriptor error, params ReadOnlySpan<ErrorGroupUri> errorGroups)
    {
        foreach (var errorGroup in errorGroups)
        {
            if (error.Matches(errorGroup))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether the error descriptor is in any of the error groups in the sequence.
    /// </summary>
    public static bool MatchesAny(this ErrorDescriptor error, IEnumerable<ErrorGroupUri> errorGroups)
    {
        foreach (var errorGroup in errorGroups)
        {
            if (error.Matches(errorGroup))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether the error descriptor has the specified error type.
    /// </summary>
    public static bool Matches(this ErrorDescriptor error, ErrorUri type) =>
        type == error.Type;

    /// <summary>
    /// Determines whether the error descriptor has any of the specified error types.
    /// </summary>
    public static bool MatchesAny(this ErrorDescriptor error, params ReadOnlySpan<ErrorUri> types)
    {
        foreach (var type in types)
        {
            if (error.Matches(type))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether the error descriptor has any of the error types in the sequence.
    /// </summary>
    public static bool MatchesAny(this ErrorDescriptor error, IEnumerable<ErrorUri> types)
    {
        foreach (var type in types)
        {
            if (error.Matches(type))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether the error descriptor has same type and status code as the specified error descriptor.
    /// </summary>
    public static bool Matches(this ErrorDescriptor error, ErrorDescriptor errorDescriptor) =>
        errorDescriptor.Type == error.Type
        && errorDescriptor.StatusCode == error.StatusCode;

    /// <summary>
    /// Determines whether the error descriptor has same type and status code as any of the specified error descriptors.
    /// </summary>
    public static bool MatchesAny(this ErrorDescriptor error, params ReadOnlySpan<ErrorDescriptor> descriptors)
    {
        foreach (var descriptor in descriptors)
        {
            if (error.Matches(descriptor))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether the error descriptor has same type and status code as any of the error descriptors in the sequence.
    /// </summary>
    public static bool MatchesAny(this ErrorDescriptor error, IEnumerable<ErrorDescriptor> descriptors)
    {
        foreach (var descriptor in descriptors)
        {
            if (error.Matches(descriptor))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether the service error has the specified status code.
    /// </summary>
    public static bool Matches(this ServiceError error, ErrorStatusCode statusCode) =>
        error.Descriptor.Matches(statusCode);

    /// <summary>
    /// Determines whether the service error has any of the specified status codes.
    /// </summary>
    public static bool MatchesAny(this ServiceError error, params ReadOnlySpan<ErrorStatusCode> statusCodes) =>
        error.Descriptor.MatchesAny(statusCodes);

    /// <summary>
    /// Determines whether the service error has any of the status codes in the sequence.
    /// </summary>
    public static bool MatchesAny(this ServiceError error, IEnumerable<ErrorStatusCode> statusCodes) =>
        error.Descriptor.MatchesAny(statusCodes);

    /// <summary>
    /// Determines whether the service error is in the specified error group.
    /// </summary>
    public static bool Matches(this ServiceError error, ErrorGroupUri errorGroup) =>
        error.Descriptor.Matches(errorGroup);

    /// <summary>
    /// Determines whether the service error's descriptor is in any of the specified error groups.
    /// </summary>
    public static bool MatchesAny(this ServiceError error, params ReadOnlySpan<ErrorGroupUri> errorGroups) =>
        error.Descriptor.MatchesAny(errorGroups);

    /// <summary>
    /// Determines whether the service error's descriptor is in any of the error groups in the sequence.
    /// </summary>
    public static bool MatchesAny(this ServiceError error, IEnumerable<ErrorGroupUri> errorGroups) =>
        error.Descriptor.MatchesAny(errorGroups);

    /// <summary>
    /// Determines whether the service error has the specified error type.
    /// </summary>
    public static bool Matches(this ServiceError error, ErrorUri type) =>
        error.Descriptor.Matches(type);

    /// <summary>
    /// Determines whether the service error's descriptor has any of the specified error types.
    /// </summary>
    public static bool MatchesAny(this ServiceError error, params ReadOnlySpan<ErrorUri> types) =>
        error.Descriptor.MatchesAny(types);

    /// <summary>
    /// Determines whether the service error's descriptor has any of the error types in the sequence.
    /// </summary>
    public static bool MatchesAny(this ServiceError error, IEnumerable<ErrorUri> types) =>
        error.Descriptor.MatchesAny(types);

    /// <summary>
    /// Determines whether the service error has same type and status code as the specified error descriptor.
    /// </summary>
    public static bool Matches(this ServiceError error, ErrorDescriptor errorDescriptor) =>
        error.Descriptor.Matches(errorDescriptor);

    /// <summary>
    /// Determines whether the service error's descriptor has same type and status code as any of the specified error
    /// descriptors.
    /// </summary>
    public static bool MatchesAny(this ServiceError error, params ReadOnlySpan<ErrorDescriptor> descriptors) =>
        error.Descriptor.MatchesAny(descriptors);

    /// <summary>
    /// Determines whether the service error's descriptor has same type and status code as any of the error descriptors in the
    /// sequence.
    /// </summary>
    public static bool MatchesAny(this ServiceError error, IEnumerable<ErrorDescriptor> descriptors) =>
        error.Descriptor.MatchesAny(descriptors);

    /// <summary>
    /// Determines whether the service exception's descriptor has the specified status code.
    /// </summary>
    public static bool Matches(this IServiceException serviceException, ErrorStatusCode statusCode) =>
        serviceException.Descriptor.Matches(statusCode);

    /// <summary>
    /// Determines whether the service exception's descriptor has any of the specified status codes.
    /// </summary>
    public static bool MatchesAny(
        this IServiceException serviceException,
        params ReadOnlySpan<ErrorStatusCode> statusCodes) =>
        serviceException.Descriptor.MatchesAny(statusCodes);

    /// <summary>
    /// Determines whether the service exception's descriptor has any of the status codes in the sequence.
    /// </summary>
    public static bool MatchesAny(this IServiceException serviceException, IEnumerable<ErrorStatusCode> statusCodes) =>
        serviceException.Descriptor.MatchesAny(statusCodes);

    /// <summary>
    /// Determines whether the service exception's descriptor is in the specified error group.
    /// </summary>
    public static bool Matches(this IServiceException serviceException, ErrorGroupUri errorGroup) =>
        serviceException.Descriptor.Matches(errorGroup);

    /// <summary>
    /// Determines whether the service exception's descriptor is in any of the specified error groups.
    /// </summary>
    public static bool MatchesAny(
        this IServiceException serviceException,
        params ReadOnlySpan<ErrorGroupUri> errorGroups) =>
        serviceException.Descriptor.MatchesAny(errorGroups);

    /// <summary>
    /// Determines whether the service exception's descriptor is in any of the error groups in the sequence.
    /// </summary>
    public static bool MatchesAny(this IServiceException serviceException, IEnumerable<ErrorGroupUri> errorGroups) =>
        serviceException.Descriptor.MatchesAny(errorGroups);

    /// <summary>
    /// Determines whether the service exception's descriptor has the specified error type.
    /// </summary>
    public static bool Matches(this IServiceException serviceException, ErrorUri type) =>
        serviceException.Descriptor.Matches(type);

    /// <summary>
    /// Determines whether the service exception's descriptor has any of the specified error types.
    /// </summary>
    public static bool MatchesAny(this IServiceException serviceException, params ReadOnlySpan<ErrorUri> types) =>
        serviceException.Descriptor.MatchesAny(types);

    /// <summary>
    /// Determines whether the service exception's descriptor has any of the error types in the sequence.
    /// </summary>
    public static bool MatchesAny(this IServiceException serviceException, IEnumerable<ErrorUri> types) =>
        serviceException.Descriptor.MatchesAny(types);

    /// <summary>
    /// Determines whether the service exception's descriptor has same type and status code as the specified error descriptor.
    /// </summary>
    public static bool Matches(this IServiceException serviceException, ErrorDescriptor errorDescriptor) =>
        serviceException.Descriptor.Matches(errorDescriptor);

    /// <summary>
    /// Determines whether the service exception's descriptor has same type and status code as any of the specified error
    /// descriptors.
    /// </summary>
    public static bool MatchesAny(
        this IServiceException serviceException,
        params ReadOnlySpan<ErrorDescriptor> descriptors) =>
        serviceException.Descriptor.MatchesAny(descriptors);

    /// <summary>
    /// Determines whether the service exception's descriptor has same type and status code as any of the error descriptors in
    /// the sequence.
    /// </summary>
    public static bool MatchesAny(this IServiceException serviceException, IEnumerable<ErrorDescriptor> descriptors) =>
        serviceException.Descriptor.MatchesAny(descriptors);
}