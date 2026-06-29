namespace CodeMe.ServiceErrors;

/// <summary>
/// Service error details.
/// </summary>
/// <param name="Descriptor">Error descriptor.</param>
/// <param name="Message">Error detail message.</param>
public readonly record struct ServiceError(
    ErrorDescriptor Descriptor,
    string Message)
{
    /// <summary>
    /// Debug-only inner exception. Not preserved on serialization.
    /// </summary>
    public Exception? InnerException { get; init; }

    /// <summary>
    /// Debug-only inner errors (obtained from <see cref="InnerException"/>).
    /// </summary>
    public IReadOnlyCollection<ServiceError> InnerErrors =>
        InnerException switch
        {
            null => [],
            IServiceException ex => [ex.Error],
            AggregateException ex => ex.Flatten().InnerExceptions.OfType<IServiceException>().Select(x => x.Error)
                .ToArray(),
            _ => []
        };

    /// <inheritdoc/>
    public bool Equals(ServiceError other) =>
        Descriptor.Equals(other.Descriptor)
        && string.Equals(Message, other.Message, StringComparison.Ordinal);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Descriptor, Message);
}