using System.ComponentModel;
using Firework.Abstraction.Services;
using Firework.Core.LowLevel.Mouse;
using Firework.Core.MacroServices.Attrubutes;
using Firework.Models.Instructions;

namespace Firework.Core.MacroServices;

public class MouseService : ServiceBase
{
    public MouseService(IServiceManager serviceManager) : base(serviceManager)
    {
        Mouse = new();
    }

    private MouseButton Mouse { get; }
    
    [ActionService(Alias = "move")]
    public void MoveMouse(List<ActionParameterInfo> parameter)
    {
        var x = Convert.ToInt32(parameter.First().Value);
        var y = Convert.ToInt32(parameter.First().Value);

        var mouse = new Move();
        mouse.MoveCursorTo(x, y);
    }

    [ActionService(Alias = "click")]
    public void ClickMouseButton(string button = "left")
    {
        var buttonEnum = (MouseButtons) Enum.Parse(typeof(MouseButtons), button, true);

        if (!Enum.IsDefined(typeof(MouseButtons), buttonEnum))
            throw new InvalidEnumArgumentException(nameof(buttonEnum), (int) buttonEnum, typeof(MouseButtons));

        switch (buttonEnum)
        {
            case MouseButtons.Left:
                Mouse.LeftButtonClick();
                break;
            case MouseButtons.Right:
                Mouse.RightButtonClick();
                break;
            case MouseButtons.Middle:
                Mouse.MiddleButtonClick();
                break;
        }
    }

    [ActionService(Alias = "hold")]
    public void HoldMouseButton(List<ActionParameterInfo> parameter)
    {
        var button = (MouseButtons) Enum.Parse(typeof(MouseButtons), parameter.First().Name, true);

        switch (button)
        {
            case MouseButtons.Left:
                Mouse.HoldLeftButton();
                break;
            case MouseButtons.Right:
                Mouse.HoldRightButton();
                break;
            case MouseButtons.Middle:
                Mouse.HoldMiddleButton();
                break;
        }
    }

    [ActionService(Alias = "release")]
    public void ReleaseMouseButton(List<ActionParameterInfo> parameter)
    {
        var button = (MouseButtons) Enum.Parse(typeof(MouseButtons), parameter.First().Name, true);

        switch (button)
        {
            case MouseButtons.Left:
                Mouse.ReleaseLeftButton();
                break;
            case MouseButtons.Right:
                Mouse.ReleaseRightButton();
                break;
            case MouseButtons.Middle:
                Mouse.ReleaseMiddleButton();
                break;
        }
    }

    private enum MouseButtons
    {
        Left,
        Right,
        Middle
    }
}