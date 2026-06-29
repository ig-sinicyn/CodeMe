using System.Reflection;

namespace CodeMe.ServiceErrors.Serializable.Builders;

/// <summary>
/// Helper utilities for building <see cref="IServiceErrorFactory"/>>.
/// </summary>
public static class ServiceErrorFactoryBuilderHelper
{
    /// <summary>
    /// Flattens assembly references starting from a root assembly and filters by name prefix.
    /// </summary>
    public static IEnumerable<Assembly> FlattenReferences(Assembly root, string? referenceNamePrefix) =>
        FlattenReferences(root, referenceNamePrefix ?? "", []);

    private static IEnumerable<Assembly> FlattenReferences(
        Assembly current,
        string referenceNamePrefix,
        HashSet<AssemblyName> visitedAssemblies)
    {
        var currentName = current.GetName();
        var isRoot = visitedAssemblies.Count == 0;

        if (!visitedAssemblies.Add(currentName))
        {
            yield break;
        }

        if (isRoot
            || (currentName.Name?.StartsWith(referenceNamePrefix, StringComparison.OrdinalIgnoreCase) ?? false))
        {
            yield return current;
        }

        foreach (var referencedAssemblyName in current.GetReferencedAssemblies())
        {
            Assembly referencedAssembly;
            try
            {
                referencedAssembly = Assembly.Load(referencedAssemblyName);
            }
            catch (IOException)
            {
                // Skip assemblies that cannot be loaded
                continue;
            }
            catch (BadImageFormatException)
            {
                // Skip assemblies that cannot be loaded
                continue;
            }

            foreach (var assembly in FlattenReferences(referencedAssembly, referenceNamePrefix, visitedAssemblies))
            {
                yield return assembly;
            }
        }
    }

    /// <summary>
    /// Gets well-known error types from an assembly optionally filtered by attribute.
    /// </summary>
    public static IEnumerable<Type> GetWellKnownErrorTypes(
        Assembly assembly,
        bool filterByServiceErrorsAttribute = true)
    {
        var types = assembly.GetTypes()
            .Where(x => x is { IsClass: true, IsAbstract: true, IsSealed: true });

        types = filterByServiceErrorsAttribute
            ? types.Where(x => x.IsDefined(typeof(ServiceErrorsAttribute), false))
            : types.Where(x => x.IsWellKnownErrorType());

        return types;
    }

    private static bool IsWellKnownErrorType(this Type candidate)
    {
        var errorFields = candidate.GetFields(BindingFlags.Public | BindingFlags.Static);

        return errorFields.Any(
            x => x.FieldType.IsAssignableTo(typeof(ErrorDescriptor))
                || x.FieldType.IsAssignableTo(typeof(ErrorGroupUri))
                || x.FieldType.IsAssignableTo(typeof(ErrorUri)));
    }

    /// <summary>
    /// Fills service error factory options from a well-known error type.
    /// </summary>
    public static void FillOptions(ServiceErrorFactoryOptions options, Type wellKnownErrorType)
    {
        var errorFields = wellKnownErrorType.GetFields(BindingFlags.Public | BindingFlags.Static);

        foreach (var mapping in errorFields)
        {
            if (mapping.FieldType.IsAssignableTo(typeof(ErrorDescriptor)))
            {
                var descriptor = (ErrorDescriptor)mapping.GetValue(null)!;
                options.ErrorCodeMapping.Add(descriptor.Type.Code, descriptor);

                if (mapping.GetCustomAttribute<ServiceExceptionAttribute>() is
                    { } descriptorAttribute)
                {
                    options.ErrorFactories.Add(descriptor.Type, GetExceptionFactory(descriptorAttribute.ExceptionType));
                }

                continue;
            }

            if (mapping.FieldType.IsAssignableTo(typeof(ErrorGroupUri))
                && mapping.GetCustomAttribute<ServiceExceptionAttribute>() is
                    { } groupAttribute)
            {
                var group = (ErrorGroupUri)mapping.GetValue(null)!;
                options.ErrorGroupFactories.Add(group, GetExceptionFactory(groupAttribute.ExceptionType));
                continue;
            }

            if (mapping.FieldType.IsAssignableTo(typeof(ErrorUri))
                && mapping.GetCustomAttribute<ServiceExceptionAttribute>() is
                    { } typeAttribute)
            {
                var type = (ErrorUri)mapping.GetValue(null)!;
                options.ErrorFactories.Add(type, GetExceptionFactory(typeAttribute.ExceptionType));
            }
        }
    }

    private static Func<ServiceError, IServiceException> GetExceptionFactory(Type exceptionType)
    {
        var factory = ConstructorInvoker.Create(
            exceptionType.GetConstructor(
                BindingFlags.Public | BindingFlags.Instance,
                [typeof(ServiceError)])!);

        return err => (IServiceException)factory.Invoke(err);
    }
}