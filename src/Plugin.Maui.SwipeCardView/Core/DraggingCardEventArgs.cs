using Microsoft.Maui.Controls;

namespace Plugin.Maui.SwipeCardView.Core;

/// <summary>Arguments for drag events raised while a card is being dragged.</summary>
public class DraggingCardEventArgs : System.EventArgs
{
    /// <summary>Creates a new instance without a card view reference.</summary>
    public DraggingCardEventArgs(object item, object parameter, SwipeCardDirection direction, DraggingCardPosition position, double distanceDraggedX, double distanceDraggedY)
        : this(item, parameter, direction, position, distanceDraggedX, distanceDraggedY, null)
    {
    }

    /// <summary>Creates a new instance with a card view reference.</summary>
    public DraggingCardEventArgs(object item, object parameter, SwipeCardDirection direction, DraggingCardPosition position, double distanceDraggedX, double distanceDraggedY, View? cardView)
    {
        Item = item;
        Parameter = parameter;
        Direction = direction;
        Position = position;
        DistanceDraggedX = distanceDraggedX;
        DistanceDraggedY = distanceDraggedY;
        CardView = cardView;
    }

    /// <summary>Gets the data item bound to the card being dragged.</summary>
    public object Item { get; }

    /// <summary>Gets the command parameter.</summary>
    public object Parameter { get; }

    /// <summary>Gets the direction of the drag.</summary>
    public SwipeCardDirection Direction { get; }

    /// <summary>Gets the current dragging position relative to the threshold.</summary>
    public DraggingCardPosition Position { get; }

    /// <summary>Gets the distance dragged on the X axis (in device-independent units).</summary>
    public double DistanceDraggedX { get; }

    /// <summary>Gets the distance dragged on the Y axis (in device-independent units).</summary>
    public double DistanceDraggedY { get; }

    /// <summary>Gets the card view being dragged. Use this to find named elements within the card's DataTemplate (e.g. <c>CardView.FindByName&lt;Label&gt;("MyLabel")</c>).</summary>
    public View? CardView { get; }
}