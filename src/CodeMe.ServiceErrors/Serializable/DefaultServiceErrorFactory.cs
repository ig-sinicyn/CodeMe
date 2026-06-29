namespace CodeMe.ServiceErrors.Serializable;

public class DefaultServiceErrorFactory : IServiceErrorFactory
{
    private readonly Func<IServiceErrorFactoryOptions> _optionsAccessor;

    public DefaultServiceErrorFactory(IServiceErrorFactoryOptions options) : this(() => options)
    {
    }

    public DefaultServiceErrorFactory(Func<IServiceErrorFactoryOptions> optionsAccessor)
    {
        _optionsAccessor = optionsAccessor;
    }

    public ErrorGroupUri RootErrorGroup => _optionsAccessor().RootErrorGroup;

    public ServiceErrorDto CreateDto(ServiceError error)
    {
        var options = _optionsAccessor();

        var errorType = error.Descriptor.Type;
        var registeredType = options.ErrorCodeMapping.GetValueOrDefault(errorType.Code)?.Type;
        var isWellKnown = errorType == registeredType;

        var scheme = GetDtoField(
            options.SchemeFillMode,
            errorType.Group.Scheme,
            options.RootErrorGroup.Scheme,
            isWellKnown);
        var application = GetDtoField(
            options.ApplicationFillMode,
            errorType.Group.Application,
            options.RootErrorGroup.Application,
            isWellKnown);
        var category = GetDtoField(
            options.CategoryFillMode,
            errorType.Group.Category,
            options.RootErrorGroup.Category,
            isWellKnown);

        return new ServiceErrorDto
        {
            Scheme = scheme,
            Application = application,
            Category = category,
            Code = errorType.Code,
            StatusCode = error.Descriptor.StatusCode,
            Message = error.Message
        };
    }

    private static string? GetDtoField(
        ErrorDtoFillMode fillMode,
        string? value,
        string? defaultValue,
        bool isWellKnown)
    {
        return fillMode switch
        {
            ErrorDtoFillMode.IfUnknown =>
                isWellKnown || string.Equals(value, defaultValue, ErrorGroupUri.UriPartComparison)
                    ? null
                    : value,
            ErrorDtoFillMode.Always => value,
            ErrorDtoFillMode.Newer => null,
            _ => throw new ArgumentOutOfRangeException(nameof(fillMode), fillMode, null)
        };
    }

    public ServiceError CreateError(ServiceErrorDto error) =>
        new(
            CreateErrorDescriptor(error),
            error.Message);

    public ServiceError CreateError(Exception exception)
    {
        var options = _optionsAccessor();

        if (exception is IServiceException serviceException)
        {
            return serviceException.Error;
        }

        var errorType = ErrorUri.Create(
            options.RootErrorGroup,
            ErrorUriConvert.ExceptionTypeToErrorCode(exception.GetType()));

        var descriptor = new ErrorDescriptor(errorType, ErrorStatusCode.Internal);

        return new ServiceError(
            descriptor,
            exception.Message)
        {
            InnerException = exception
        };
    }

    private ErrorDescriptor CreateErrorDescriptor(ServiceErrorDto error)
    {
        var options = _optionsAccessor();

        if (options.ErrorCodeMapping.TryGetValue(error.Code, out var descriptorByCode))
        {
            var byCodeGroup = descriptorByCode.Type.Group;
            var sameGroup = DtoFieldMatch(error.Scheme, byCodeGroup.Scheme)
                && DtoFieldMatch(error.Application, byCodeGroup.Application)
                && DtoFieldMatch(error.Category, byCodeGroup.Category);
            if (sameGroup)
            {
                return descriptorByCode;
            }
        }

        var rootGroup = options.RootErrorGroup;
        var sameApp = DtoFieldMatch(error.Scheme, rootGroup.Scheme)
            && DtoFieldMatch(error.Application, rootGroup.Application);
        var errorGroup = ErrorGroupUri.Create(
            error.Scheme ?? rootGroup.Scheme,
            error.Application ?? rootGroup.Application,
            sameApp ? error.Category ?? rootGroup.Category : error.Category);

        var statusCode = error.StatusCode == 0 ? ErrorStatusCode.Internal : error.StatusCode;

        return ErrorDescriptor.Create(
            errorGroup,
            error.Code,
            statusCode);
    }

    private static bool DtoFieldMatch(string? dtoField, string? expected) =>
        dtoField == null || string.Equals(dtoField, expected, ErrorGroupUri.UriPartComparison);

    public IServiceException CreateException(ServiceError error)
    {
        var options = _optionsAccessor();

        // Get factory by type
        if (options.ErrorFactories.TryGetValue(error.Descriptor.Type, out var factory))
        {
            return factory(error);
        }

        // Get factory by group
        var errorGroupFactories = options.ErrorGroupFactories;
        if (errorGroupFactories.Count > 0)
        {
            var currentGroup = error.Descriptor.Type.Group;
            while (currentGroup != null)
            {
                if (errorGroupFactories.TryGetValue(currentGroup, out var groupFactory))
                {
                    return groupFactory(error);
                }

                currentGroup = currentGroup.ParentGroup;
            }
        }

        // Default factory
        return options.DefaultErrorFactory?.Invoke(error)
            ?? new ServiceException(error);
    }
}