namespace Plugin.Maui.SwipeCardView.Core;

/// <summary>Arguments for swipe events raised when a card is swiped past the threshold.</summary>
public class SwipedCardEventArgs : System.EventArgs
{
    public SwipedCardEventArgs(object item, object parameter,
        SwipeCardDirection direction)
    {
        Item = item;
        Parameter = parameter;
        Direction = direction;
    }

    /// <summary>Gets the data item bound to the swiped card.</summary>
    public object Item { get; }

    /// <summary>Gets the command parameter.</summary>
    public object Parameter { get; }

    /// <summary>Gets the direction the card was swiped.</summary>
    public SwipeCardDirection Direction { get; }
}