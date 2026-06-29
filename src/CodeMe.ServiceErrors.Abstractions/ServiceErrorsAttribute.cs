using CodeMe.ServiceErrors.Serializable;

namespace CodeMe.ServiceErrors;

/// <summary>
/// Marks a static class containing error details for configuring <see cref="IServiceErrorFactory"/> error and exception
/// mappings.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ServiceErrorsAttribute : Attribute
{
}