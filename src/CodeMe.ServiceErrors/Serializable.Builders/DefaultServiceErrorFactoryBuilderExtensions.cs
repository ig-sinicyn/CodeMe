using System.Reflection;
using static CodeMe.ServiceErrors.Serializable.Builders.ServiceErrorFactoryBuilderHelper;

namespace CodeMe.ServiceErrors.Serializable.Builders;

public static class DefaultServiceErrorFactoryBuilderExtensions
{
    /// <summary>
    /// Adds well-known errors defined in the specified type <paramref name="wellKnownErrorsType"/>.
    /// </summary>
    public static DefaultServiceErrorFactoryBuilder Add(
        this DefaultServiceErrorFactoryBuilder builder,
        Type wellKnownErrorsType) =>
        builder.Configure(opt => FillOptions(opt, wellKnownErrorsType));

    /// <summary>
    /// Adds well-known errors defined in the specified <paramref name="assembly"/>.
    /// </summary>
    public static DefaultServiceErrorFactoryBuilder AddAssembly(
        this DefaultServiceErrorFactoryBuilder builder,
        Assembly assembly,
        bool filterByServiceErrorsAttribute = true) =>
        builder.Configure(
            opt =>
            {
                foreach (var wellKnownErrorType in GetWellKnownErrorTypes(assembly, filterByServiceErrorsAttribute))
                {
                    FillOptions(opt, wellKnownErrorType);
                }
            });

    /// <summary>
    /// Adds well-known errors defined in the specified <paramref name="root"/> assembly and referenced assemblies.
    /// </summary>
    public static DefaultServiceErrorFactoryBuilder AddAssemblyAndDependencies(
        this DefaultServiceErrorFactoryBuilder builder,
        Assembly root,
        string referenceNamePrefix,
        bool filterByServiceErrorsAttribute = true) =>
        builder.Configure(
            opt =>
            {
                var types = FlattenReferences(root, referenceNamePrefix)
                    .SelectMany(x => GetWellKnownErrorTypes(x, filterByServiceErrorsAttribute));
                foreach (var wellKnownErrorType in types)
                {
                    FillOptions(opt, wellKnownErrorType);
                }
            });
}