using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using CodeMe.ServiceErrors.Serializable;

namespace CodeMe.ServiceErrors.DependencyInjection;

using OptionsAccessor = Func<IServiceErrorFactoryOptions>;
using FactoryCallback = Func<Func<IServiceErrorFactoryOptions>, IServiceErrorFactory>;

internal static class ServiceErrorFactoryEmitter
{
    private readonly record struct FactoryTypes(Type Contract, Type Implementation);

    private static readonly Lazy<ModuleBuilder> _moduleBuilder = new(
        () =>
        {
            var assemblyName = new AssemblyName($"{nameof(ServiceErrorFactoryEmitter)}_{Guid.NewGuid()}");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            return assemblyBuilder.DefineDynamicModule(assemblyName.Name!);
        });

    private static readonly ConcurrentDictionary<FactoryTypes, FactoryCallback> _emittedFactories = new();

    public static Func<OptionsAccessor, TErrorFactory> CreateCallback<TErrorFactory>(Type? implementationType = null)
        where TErrorFactory : class, IServiceErrorFactory
    {
        implementationType ??= typeof(DefaultServiceErrorFactory);

        var contractType = typeof(TErrorFactory);
        if (!contractType.IsInterface)
        {
            throw new ArgumentException($"The {contractType.Name} type should be an interface");
        }

        if (contractType == typeof(IServiceErrorFactory) && implementationType == typeof(DefaultServiceErrorFactory))
        {
            FactoryCallback callback = static opt => new DefaultServiceErrorFactory(opt);
            return (Func<OptionsAccessor, TErrorFactory>)callback;
        }

        // NB: the EmitFactoryCallbackCore() MAY be called multiple times on parallel calls,
        // but probability is low as the factory will be registered as singleton so let's ignore it.
        return (Func<OptionsAccessor, TErrorFactory>)_emittedFactories.GetOrAdd(
            new FactoryTypes(contractType, implementationType),
            static x =>
            {
                var emittedType = EmitFactoryType(x.Contract, x.Implementation);

                return CreateCallbackForFactoryType<TErrorFactory>(emittedType);
            });
    }

    private static Type EmitFactoryType(Type contractType, Type implementationType)
    {
        var typeBuilder = _moduleBuilder.Value.DefineType(
            $"{contractType.FullName}_Implementation_{Guid.NewGuid()}",
            TypeAttributes.Public | TypeAttributes.Sealed,
            implementationType);
        typeBuilder.AddInterfaceImplementation(contractType);

        // Define constructor that takes Func<IServiceErrorFactoryOptions>
        var constructorBuilder = typeBuilder.DefineConstructor(
            MethodAttributes.Public,
            CallingConventions.Standard,
            [typeof(Func<IServiceErrorFactoryOptions>)]);

        var il = constructorBuilder.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0); // Load 'this'
        il.Emit(OpCodes.Ldarg_1); // Load the Func<IServiceErrorFactoryOptions> parameter

        var baseConstructor = implementationType
            .GetConstructor(
                BindingFlags.Public | BindingFlags.Instance,
                [typeof(Func<IServiceErrorFactoryOptions>)])!;
        il.Emit(OpCodes.Call, baseConstructor);
        il.Emit(OpCodes.Ret);

        var emittedType = typeBuilder.CreateType();

        return emittedType;
    }

    private static Func<OptionsAccessor, TErrorFactory> CreateCallbackForFactoryType<TErrorFactory>(
        Type emittedFactoryType)
        where TErrorFactory : class, IServiceErrorFactory
    {
        var factory = ConstructorInvoker.Create(
            emittedFactoryType.GetConstructor(
                BindingFlags.Public | BindingFlags.Instance,
                [typeof(Func<IServiceErrorFactoryOptions>)])!);

        return opt => (TErrorFactory)factory.Invoke(opt);
    }
}