using System.Runtime.InteropServices;

namespace Firework.Core.LowLevel.Mouse;

public class Move
{
    public enum MoveCursor
    {
        UpperLeftCorner,
        UpperRightCorner,
        BottomLeftCorner,
        BottomRightCorner,
        LeftMiddle,
        RightMiddle,
        TopMiddle,
        BottomMiddle,
        Center
    }

    private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
    private const uint MOUSEEVENTF_MOVE = 0x0001;

    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlag, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

    public void MoveCursorTo(int x, int y)
    {
        mouse_event(MOUSEEVENTF_MOVE, x, y, 0, (UIntPtr) 0);
    }

    public void MoveCursorTo(MoveCursor to)
    {
        mouse_event(MOUSEEVENTF_MOVE, 100, 0, 0, UIntPtr.Zero);
    }
}