/*using Firework.Abstraction.Instruction;
using Firework.Abstraction.MacroLauncher;
using Firework.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Firework.Core.Test.Tests.Instructions;

[TestFixture]
public class MacroLauncherTest
{
    private IServiceProvider _serviceProvider;
    private IMacroLauncher _macroLauncher;
    private IInstructionService _instructionService;

    [SetUp]
    public void SetUp()
    {
        var configServices = new ConfigureServices();

        _serviceProvider = configServices.ServiceProvider;
        _macroLauncher = _serviceProvider.GetService<IMacroLauncher>();
        _instructionService = _serviceProvider.GetService<IInstructionService>();
    }


    [TestCase("test>test")]
    [TestCase("test>test()")]
    [TestCase("test>TestWithParams(param1: test, param2: 123)")]
    [TestCase("test>TestWithParams_Alias(param1: test, param2: 123)")]
    public void CreateInstruction_TestActionWithAndWithoutParams_ResultSuccess(string instruction)
    {
        var instructionInfo = _instructionService.CreateInstruction(instruction);
        var result = _macroLauncher.Start(instructionInfo);

        Assert.That(result, Is.EqualTo("success"));
    }

    [TestCase("test>testvoid")]
    [TestCase("test>testvoid()")]
    [TestCase("test>TestVoidAlias_Alias()")]
    [TestCase("test>TestVoidAlias_Alias()")]
    public void Start_TestVoidActions_EmptyString(string instruction)
    {
        var instructionInfo = _instructionService.CreateInstruction(instruction);
        var result = _macroLauncher.Start(instructionInfo);

        Assert.That(result, Is.EqualTo(String.Empty));
    }


    [TestCase("test>TestPrivateWithAliasAndParam(test: test)")]
    [TestCase("test>TestPrivateWithAliasAndParam()")]
    [TestCase("test>TestPrivateWithAliasAndParam_Alias(test: test)")]
    [TestCase("test>TestPrivateWithAliasAndParam_Alias()")]
    [TestCase("test>TestPrivateWithAlias()")]
    [TestCase("test>TestPrivateWithAlias")]
    [TestCase("test>TestPrivateWithAlias_Alias")]
    [TestCase("test>TestPrivateWithAlias_Alias()")]
    [TestCase("test>testprivate")]
    [TestCase("test>testprivate()")]
    [TestCase("test>testvoidalias")]
    [TestCase("test>testvoidalias()")]
    [TestCase("test>TestWithoutAttribute")]
    [TestCase("test>TestWithoutAttribute()")]
    [TestCase("test>TestVoidWithoutAttribute")]
    [TestCase("test>TestVoidWithoutAttribute()")]
    [TestCase("test>TestWithParamsWithoutAttribute()")]
    [TestCase("test>TestWithParamsWithoutAttribute(param1: test, param2: test2)")]
    [TestCase("test>TestWithParamsAlias(param1: test, param2: test2)")]
    public void Start_OutOfAreaActions_Exception(string instruction)
    {
        Assert.Throws<ParseInstructionException>(() => _instructionService.CreateInstruction(instruction));
    }
}*/