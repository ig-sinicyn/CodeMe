namespace CodeMe.ServiceErrors;

/// <summary>
/// Marks a field of type <see cref="ErrorDescriptor"/>, <see cref="ErrorUri"/>, <see cref="ErrorGroupUri"/>
/// with associated exception.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ServiceExceptionAttribute<TException> : ServiceExceptionAttribute
    where TException : Exception, IServiceException
{
    /// <summary>
    /// Marks a field of type <see cref="ErrorDescriptor"/>, <see cref="ErrorUri"/>, <see cref="ErrorGroupUri"/>
    /// with associated exception.
    /// </summary>
    public ServiceExceptionAttribute() : base(typeof(TException))
    {
    }
}