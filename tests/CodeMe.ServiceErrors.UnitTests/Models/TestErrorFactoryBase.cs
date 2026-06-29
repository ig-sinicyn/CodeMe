using CodeMe.ServiceErrors.Serializable;

namespace CodeMe.ServiceErrors.UnitTests.Models;

public class TestErrorFactoryBase : DefaultServiceErrorFactory
{
    public TestErrorFactoryBase(Func<IServiceErrorFactoryOptions> optionsAccessor) : base(optionsAccessor)
    {
    }
}