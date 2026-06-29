namespace CodeMe.ServiceErrors;

/// <summary>
/// Marks a field of type <see cref="ErrorDescriptor"/>, <see cref="ErrorUri"/>, <see cref="ErrorGroupUri"/>
/// with associated exception.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ServiceExceptionAttribute : Attribute
{
    /// <summary>
    /// Marks a field of type <see cref="ErrorDescriptor"/>, <see cref="ErrorUri"/>, <see cref="ErrorGroupUri"/>
    /// with associated exception.
    /// </summary>
    public ServiceExceptionAttribute(Type exceptionType)
    {
        ExceptionType = exceptionType;
    }

    /// <summary>
    /// Exception type to be used for the specified description / type / error group.
    /// </summary>
    public Type ExceptionType { get; }
}