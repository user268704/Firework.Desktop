/*using Firework.Abstraction.Instruction;
using Firework.Abstraction.MacroLauncher;
using Firework.Core.DI;
using Firework.Core.Exceptions;
using Firework.Core.Instruction;
using Firework.Core.Services;
using Firework.Models.Instructions;
using Microsoft.Extensions.DependencyInjection;

namespace Firework.Core.Test.Tests.Instructions;

[TestFixture]
public class InstructionServiceTests
{
 
    private IServiceProvider _serviceProvider;
    private IInstructionService _instructionService;

    [SetUp]
    public void SetUp()
    {
        var configServices = new ConfigureServices();

        _serviceProvider = configServices.ServiceProvider;
        _instructionService = _serviceProvider.GetService<IInstructionService>();

    }

    [Test]
    public void CreateInstruction_ValidInstruction_ReturnsInstructionInfo()
    {
        // Arrange
        var notValidInstruction1 = "task>run(cmd.exe)";
        var validInstruction1 = "task>run(path: cmd.exe)";
        var validInstruction2 = "task>run()";

        // Act
        var result = _instructionService.CreateInstruction(validInstruction1);
        var result2 = _instructionService.CreateInstruction(validInstruction2);

        // Assert
        Assert.Throws<ParseInstructionException>(() => _instructionService.CreateInstruction(notValidInstruction1));
        Assert.IsNotNull(result);
        Assert.That(result.ServiceName, Is.EqualTo("task"));
        Assert.That(result.ActionInfo.Name, Is.EqualTo("run"));


        Assert.That(result2.ServiceName, Is.EqualTo("task"));
        Assert.That(result2.ActionInfo.Name, Is.EqualTo("run"));
        Assert.That(result2.Parameters, Is.Empty);

    }

    [Test]
    public void ToString_ValidConvert_ReturnsInstructionString()
    {
        // Arrange
        var notValidInstructionString1 = "task>run(cmd.exe)";
        var instructionString1 = "task>run()";
        var instructionString2 = "task>run(path: cmd.exe, arguments: vivaldi.exe)";
        var instructionString3 = "task>run";
        var instructionString4 = "task>run(path: cmd.exe, arguments: 1, test: 1)";


        Assert.Throws<ParseInstructionException>(() => _instructionService.CreateInstruction(notValidInstructionString1));

        // Act
        var instruction1 = _instructionService.CreateInstruction(instructionString1);
        var instruction2 = _instructionService.CreateInstruction(instructionString2);
        var instruction3 = _instructionService.CreateInstruction(instructionString3);
        var instruction4 = _instructionService.CreateInstruction(instructionString4);

        // Assert
        var instructionConvertedToString1 = _instructionService.ToString(instruction1);
        var instructionConvertedToString2 = _instructionService.ToString(instruction2);
        var instructionConvertedToString3 = _instructionService.ToString(instruction3);
        var instructionConvertedToString4 = _instructionService.ToString(instruction4);

        Assert.That(instructionConvertedToString1, Is.EqualTo(instructionString1));
        Assert.That(instructionConvertedToString2, Is.EqualTo(instructionString2));
        Assert.That(instructionConvertedToString4, Is.EqualTo(instructionString4));

        // Потому что 3 и 1 это эквивалентные записи
        Assert.That(instructionConvertedToString3, Is.EqualTo(instructionString1));
    }
    
    

    [TearDown]
    public void TearDown()
    {
        if (_serviceProvider != null)
        {
            _serviceProvider = null;
        }
    }
}*/