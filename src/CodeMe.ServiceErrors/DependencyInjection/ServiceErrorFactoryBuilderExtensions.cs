using System.Reflection;
using CodeMe.ServiceErrors.Serializable;
using Microsoft.Extensions.DependencyInjection;
using static CodeMe.ServiceErrors.Serializable.Builders.ServiceErrorFactoryBuilderHelper;

namespace CodeMe.ServiceErrors.DependencyInjection;

/// <summary>
/// DI extension methods for registering service error factory and well-known errors in the DI container.
/// </summary>
public static class ServiceErrorFactoryBuilderExtensions
{
    /// <summary>
    /// Register the default implementation of <see cref="IServiceErrorFactory"/> in the DI container.
    /// Returns a builder to allow further configuration of the service error factory.
    /// </summary>
    public static IServiceErrorFactoryBuilder AddServiceErrors(
        this IServiceCollection services,
        ErrorGroupUri rootErrorGroup) =>
        services.AddServiceErrors<IServiceErrorFactory>(rootErrorGroup);

    /// <summary>
    /// Register a typed <typeparamref name="TErrorFactory"/> in the DI container.
    /// Returns a builder to allow further configuration of the service error factory.
    /// </summary>
    /// <typeparam name="TErrorFactory">Marker interface for typed error factory.</typeparam>
    /// <remarks>
    /// The <paramref name="implementationType"/>, if specified, should implement <see cref="IServiceErrorFactory"/>
    /// and should have a public constructor that accepts an <c>Func&lt;IServiceErrorFactoryOptions&gt;</c> as a parameter.
    /// </remarks>
    public static IServiceErrorFactoryBuilder AddServiceErrors<TErrorFactory>(
        this IServiceCollection services,
        ErrorGroupUri rootErrorGroup,
        Type? implementationType = null)
        where TErrorFactory : class, IServiceErrorFactory =>
        new ServiceErrorFactoryBuilder<TErrorFactory>(services, rootErrorGroup, implementationType);

    /// <summary>
    /// Adds well-known errors defined in the specified type <paramref name="wellKnownErrorsType"/>.
    /// </summary>
    public static IServiceErrorFactoryBuilder Add(
        this IServiceErrorFactoryBuilder builder,
        Type wellKnownErrorsType) =>
        builder.Configure(opt => FillOptions(opt, wellKnownErrorsType));

    /// <summary>
    /// Adds well-known errors defined in the specified <paramref name="assembly"/>.
    /// </summary>
    public static IServiceErrorFactoryBuilder AddAssembly(
        this IServiceErrorFactoryBuilder builder,
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
    public static IServiceErrorFactoryBuilder AddAssemblyAndDependencies(
        this IServiceErrorFactoryBuilder builder,
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