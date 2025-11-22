using System.Runtime.InteropServices;

namespace Firework.Core.LowLevel.Mouse;

public class MouseButton
{
    private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;
    private const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
    private const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
    private const uint MOUSEEVENTF_MOVE = 0x0001;
    private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
    private const uint MOUSEEVENTF_RIGHTUP = 0x0010;

    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlag, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

    #region Left

    public void LeftButtonClick()
    {
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, (UIntPtr) 0);
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, (UIntPtr) 0);
    }

    public void HoldLeftButton()
    {
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, (UIntPtr) 0);
    }

    public void ReleaseLeftButton()
    {
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, (UIntPtr) 0);
    }

    #endregion

    #region Right

    public void RightButtonClick()
    {
        mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, (UIntPtr) 0);
        mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, (UIntPtr) 0);
    }

    public void HoldRightButton()
    {
        mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, (UIntPtr) 0);
    }

    public void ReleaseRightButton()
    {
        mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, (UIntPtr) 0);
    }

    #endregion

    #region Middle

    public void MiddleButtonClick()
    {
        mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, UIntPtr.Zero);
        mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, UIntPtr.Zero);
    }

    public void HoldMiddleButton()
    {
        mouse_event(MOUSEEVENTF_MIDDLEDOWN, 0, 0, 0, UIntPtr.Zero);
    }

    public void ReleaseMiddleButton()
    {
        mouse_event(MOUSEEVENTF_MIDDLEUP, 0, 0, 0, UIntPtr.Zero);
    }

    #endregion
}