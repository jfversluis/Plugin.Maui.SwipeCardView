using Plugin.Maui.SwipeCardView.Core;

namespace Plugin.Maui.SwipeCardView;

internal static class SwipeCardDirectionExtensions
{
    public static bool IsLeft(this SwipeCardDirection self)
    {
        return (self & SwipeCardDirection.Left) == SwipeCardDirection.Left;
    }

    public static bool IsRight(this SwipeCardDirection self)
    {
        return (self & SwipeCardDirection.Right) == SwipeCardDirection.Right;
    }

    public static bool IsUp(this SwipeCardDirection self)
    {
        return (self & SwipeCardDirection.Up) == SwipeCardDirection.Up;
    }

    public static bool IsDown(this SwipeCardDirection self)
    {
        return (self & SwipeCardDirection.Down) == SwipeCardDirection.Down;
    }

    public static bool IsSupported(this SwipeCardDirection self, SwipeCardDirection other)
    {
        return (self & other) == other;
    }
}