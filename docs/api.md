# SwipeCardView API

SwipeCardView inherits from `Microsoft.Maui.Controls.ContentView` and implements `IDisposable`.

## Properties

Below are the properties for the SwipeCardView:

Property | Type | Default | Description
--- | --- | --- | ---
AnimationLength | `uint` | 250 | The duration in milliseconds of the animation that occurs at the end of dragging movement
BackCardScale | `float` | 0.8f | Scaling of back card while the front is being dragged. Must be between 0 (exclusive) and 1 (inclusive)
CardRotation | `float` | 20 | Rotation adjuster in degrees for dragging to left or right. From 0 (no rotation) to 360 (full circle)
DraggingCommand | `System.Windows.Input.ICommand`  | null | Gets or sets the command to run when a dragging gesture is recognized. Receives `DraggingCardEventArgs` as parameter (or `DraggingCommandParameter` if set)
DraggingCommandParameter | `System.Object` | null | Gets or sets the parameter to pass to the DraggingCommand
ItemsSource | `System.Collections.IList` | null | Gets or sets the source of items to template and display. Supports `ObservableCollection<T>` with full Add/Remove/Replace/Move/Reset handling
ItemTemplate | `Microsoft.Maui.Controls.DataTemplate` | null | Gets or sets the DataTemplate to apply to the ItemsSource. Supports any View as root, including `Border`
LoopCards | `bool` | false | Gets or sets whether the card stack loops back to the first card after reaching the end
PreviousItem | `System.Object` | null | Gets the previously swiped item (read-only, OneWayToSource). Cleared on collection Reset or when all items are removed
StackDepth | `int` | 0 | Number of cards visible behind the top card. 0 = off (backward compatible), 1 = back card visible, 2+ = back card plus shadow cards
StackOffset | `double` | 10 | Vertical offset in device-independent units between each stacked card
StackScaleStep | `double` | 0.03 | Scale reduction applied to each successive card in the stack (e.g., 0.03 = each card 3% smaller)
SupportedDraggingDirections | `SwipeCardDirection` | Left, Right, Up, Down | Gets or sets supported dragging direction of the top card
SupportedSwipeDirections | `SwipeCardDirection` | Left, Right, Up, Down | Gets or sets direction in which top card could be swiped
SwipedCommand | `System.Windows.Input.ICommand`  | null | Gets or sets the command to run when a swipe gesture is recognized. Receives `SwipedCardEventArgs` as parameter (or `SwipedCommandParameter` if set)
SwipedCommandParameter | `System.Object` | null | Gets or sets the parameter to pass to the SwipedCommand
Threshold | `uint` | 100 | Gets or sets the minimum card dragging distance that will cause the swipe gesture to be recognized
TopItem | `System.Object` | null | Gets the top item (read-only, OneWayToSource)

## Events

Event | Arguments type | Description
--- | --- | ---
Swiped | `SwipedCardEventArgs` | Raised when a card is swiped past the threshold
Dragging | `DraggingCardEventArgs` | Raised continuously during the dragging movement

### EventArgs

#### SwipedCardEventArgs

Property | Type | Description
-- | -- | --
Item | `System.Object` | Gets the item that was swiped
Parameter | `System.Object` | Gets the command parameter
Direction | `SwipeCardDirection` | Gets the direction of the swipe

#### DraggingCardEventArgs

Property | Type | Description
-- | -- | --
Item | `System.Object` | Gets the item being dragged
Parameter | `System.Object` | Gets the command parameter
Direction | `SwipeCardDirection` | Gets the direction of the drag
Position | `DraggingCardPosition` | Gets the dragging position
DistanceDraggedX | `double` | Gets the distance dragged on X axis
DistanceDraggedY | `double` | Gets the distance dragged on Y axis
CardView | `View` | Gets the card View being dragged (useful for accessing template elements)

## Methods

### InvokeSwipe(SwipeCardDirection)

Simulates a swipe gesture to the provided direction with default parameters.

Declaration

`public Task InvokeSwipe(SwipeCardDirection swipeCardDirection)`

Parameters

Type | Name | Description
--- | --- | ---
SwipeCardDirection | swipeCardDirection | Direction of the swipe

### InvokeSwipe(SwipeCardDirection, UInt32, UInt32, TimeSpan, TimeSpan)

Simulates PanGesture movement to the provided direction with full control over the animation.

Declaration

`public Task InvokeSwipe(SwipeCardDirection swipeCardDirection, uint numberOfTouches, uint touchDifferenceX, TimeSpan touchDelay, TimeSpan endTouch)`

Parameters

Type | Name | Description
--- | --- | ---
SwipeCardDirection | swipeCardDirection | Direction of the movement. Currently supported Left and Right.
System.UInt32 | numberOfTouches | Number of touch events. It should be a positive number (i.e. 20)
System.UInt32 |touchDifferenceX | Distance passed between two touches. It should be a positive number (i.e. 10)
System.TimeSpan | touchDelay | Delay between two touches. It should be a positive number (i.e. 1 millisecond)
System.TimeSpan | endDelay | End delay. It should be a positive number (i.e. 200 milliseconds)

### GoBack()

Navigates back to the previously swiped card without animation.

Declaration

`public bool GoBack()`

Returns: `true` if navigation succeeded, `false` if there is no previous card.

### GoBack(Boolean)

Navigates back to the previously swiped card with optional slide-in animation.

Declaration

`public bool GoBack(bool animated)`

Parameters

Type | Name | Description
--- | --- | ---
System.Boolean | animated | Whether to animate the card sliding in from the left

Returns: `true` if navigation succeeded, `false` if there is no previous card.

### Dispose()

Releases all resources used by the SwipeCardView. Cancels pending animations, removes gesture recognizers, and unsubscribes from collection change events.

Declaration

`public void Dispose()`

## Enums

### SwipeCardDirection

Enumerates swipe directions.

This enumeration has a [FlagsAttribute](https://learn.microsoft.com/dotnet/api/system.flagsattribute) attribute that allows a bitwise combination of its member values.

Name | Value | Description
--- | --- | ---
None | 0 | Indicates an unknown direction
Right | 1 | Indicates a rightward swipe
Left | 2 | Indicates a leftward swipe
Up | 4 | Indicates an upward swipe
Down | 8 | Indicates a downward swipe

### DraggingCardPosition

Enumerates dragging directions.

This enumeration has a [FlagsAttribute](https://learn.microsoft.com/dotnet/api/system.flagsattribute) attribute that allows a bitwise combination of its member values.

Name | Value | Description
--- | --- | ---
Start | 0 | Indicates a starting position
UnderThreshold | 1 | Indicates a position under threshold
OverThreshold | 2 | Indicates a position over threshold
FinishedUnderThreshold | 4 | Indicates an ending position under threshold
FinishedOverThreshold | 8 | Indicates an ending position over threshold