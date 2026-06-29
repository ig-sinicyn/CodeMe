namespace CodeMe.ServiceErrors;

/// <summary>
/// Service error transience.
/// </summary>
public enum ErrorTransience
{
    /// <summary>
    /// Normal user error (invalid arguments, invalid id, etc.).
    /// </summary>
    Normal,

    /// <summary>
    /// Transient error that may be resolved with retry.
    /// Examples: external service unavailability, network failure.
    /// </summary>
    Transient,

    /// <summary>
    /// Serious error that will not recover by itself.
    /// Examples: state corruption, memory leak.
    /// </summary>
    CorruptedState
}