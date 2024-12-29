using System.Diagnostics;
using Firework.Abstraction.Services;
using Firework.Core.MacroServices.Attrubutes;

namespace Firework.Core.MacroServices;

public class TestService : ServiceBase
{
    public TestService(IServiceManager serviceManager) : base(serviceManager)
    {
    }

    // v
    [ActionService]
    public string Test()
    {
        return "success";
    }

    // v
    [ActionService]
    public void TestVoid()
    {

    }

    // v
    [ActionService(Alias = "TestVoidAlias_Alias")]
    public void TestVoidAlias()
    {

    }

    // v
    public string TestWithoutAttribute()
    {
        return "without attribute";
    }

    // v
    public void TestVoidWithoutAttribute()
    {

    }

    // v
    public string TestWithParamsWithoutAttribute(string param1, string param2)
    {
        return $"without attribute but with params: 1: {param1}, 2: {param2}";
    }

    // v
    [ActionService]
    public string TestWithParams(string param1, string param2)
    {
        if (param1 == "test" && param2 == "123")
            return "success";

        return "failed";
    }

    // v
    [ActionService(Alias = "TestWithParams_Alias")]
    public string TestWithParamsAlias(string param1, string param2)
    {
        if (param1 == "test" && param2 == "123")
            return "success";

        return "failed";
    }

    // v
    [ActionService]
    private string TestPrivate()
    {
        return "success";
    }

    // v
    [ActionService(Alias = "TestPrivateWithAlias_Alias")]
    private string TestPrivateWithAlias()
    {
        return "success";
    }

    [ActionService(Alias = "TestPrivateWithAliasAndParam_Alias")]
    private string TestPrivateWithAliasAndParam(string test)
    {
        return $"private alias with param: {test}";
    }
}