namespace CodeMe.ServiceErrors.Serializable;

/// <summary>
/// Fill mode for <see cref="ServiceErrorDto"/> fields.
/// </summary>
public enum ErrorDtoFillMode
{
    /// <summary>
    /// Fill for unknown error codes only.
    /// </summary>
    IfUnknown,

    /// <summary>
    /// Always fill.
    /// </summary>
    Always,

    /// <summary>
    /// Do not fill.
    /// </summary>
    Newer
}