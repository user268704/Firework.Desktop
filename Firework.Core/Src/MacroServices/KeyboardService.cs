using Firework.Abstraction.Services;
using Firework.Core.LowLevel.Keyboard;
using Firework.Core.MacroServices.Attrubutes;
using Firework.Dto.Instructions;

namespace Firework.Core.MacroServices;

public class KeyboardService : ServiceBase
{
    public KeyboardService(IServiceManager serviceManager) : base(serviceManager)
    {
    }

    public override string Start(InstructionInfo instruction)
    {
        foreach (var parameter in instruction.Parameters)
            try
            {
                var code = (ConsoleKey) Enum.Parse(typeof(ConsoleKey), FirstLetterInUppercase(parameter.Name));

                if (int.TryParse(parameter.Value, out var numberPressed))
                {
                    for (var i = 0; i < numberPressed; i++) PressKey(code);
                    continue;
                }

                PressKey(code);
            }
            catch (ArgumentException)
            {
                var keyword = parameter.Name.ToLower();

                if (keyword == "shift") PressKey(Input.VK_SHIFT);
                if (keyword == "control") PressKey(Input.VK_CONTROL);
                if (keyword == "menu") PressKey(Input.VK_MENU);
                if (keyword == "capital") PressKey(Input.VK_CAPITAL);
            }
            catch (InvalidCastException)
            {
                /* Do nothing */
            }

        return null;
    }

    private string FirstLetterInUppercase(string text)
    {
        if (text.Length > 0)
        {
            text = char.ToUpper(text[0]) + text.Substring(1);
            return text;
        }

        return string.Empty;
    }

    [ActionService]
    public void PressKey(ConsoleKey keyCode)
    {
        var keyboard = new Input();
        keyboard.Key(keyCode);
    }

    [ActionService]
    public void PressKey(byte keyCode)
    {
        var keyboard = new Input();
        keyboard.Key(keyCode);
    }
}