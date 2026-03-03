using Plugin.Maui.SwipeCardView.Core;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Input;

namespace Plugin.Maui.SwipeCardView;

public class SwipeCardView : ContentView, IDisposable
{
    #region Bindable Properties

    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IList),
            typeof(SwipeCardView),
            null,
            propertyChanged: OnItemsSourcePropertyChanged);

    public static readonly BindableProperty ItemTemplateProperty =
        BindableProperty.Create(
            nameof(ItemTemplate),
            typeof(DataTemplate),
            typeof(SwipeCardView),
            propertyChanged: OnItemTemplatePropertyChanged);

    public static readonly BindableProperty TopItemProperty =
        BindableProperty.Create(
            nameof(TopItem),
            typeof(object),
            typeof(SwipeCardView),
            null,
            BindingMode.OneWayToSource);

    public static readonly BindableProperty PreviousItemProperty =
        BindableProperty.Create(
            nameof(PreviousItem),
            typeof(object),
            typeof(SwipeCardView),
            null,
            BindingMode.OneWayToSource);

    public static readonly BindableProperty SwipedCommandProperty =
        BindableProperty.Create(
            nameof(SwipedCommand),
            typeof(ICommand),
            typeof(SwipeCardView));

    public static readonly BindableProperty SwipedCommandParameterProperty =
        BindableProperty.Create(
            nameof(SwipedCommandParameter),
            typeof(object),
                typeof(SwipeCardView));

    public static readonly BindableProperty DraggingCommandProperty =
        BindableProperty.Create(
            nameof(DraggingCommand),
            typeof(ICommand),
            typeof(SwipeCardView));

    public static readonly BindableProperty DraggingCommandParameterProperty =
        BindableProperty.Create(
            nameof(DraggingCommandParameter),
            typeof(object),
            typeof(SwipeCardView));

    public static readonly BindableProperty ThresholdProperty =
        BindableProperty.Create(
            nameof(Threshold),
            typeof(uint),
            typeof(SwipeCardView),
            DefaultThreshold);

    public static readonly BindableProperty SupportedSwipeDirectionsProperty =
        BindableProperty.Create(
            nameof(SupportedSwipeDirections),
            typeof(SwipeCardDirection),
            typeof(SwipeCardView),
            DefaultSupportedSwipeDirections);

    public static readonly BindableProperty SupportedDraggingDirectionsProperty =
        BindableProperty.Create(
            nameof(SupportedDraggingDirections),
            typeof(SwipeCardDirection),
            typeof(SwipeCardView),
            DefaultSupportedDraggingDirections);

    public static readonly BindableProperty BackCardScaleProperty =
        BindableProperty.Create(
            nameof(BackCardScale),
            typeof(float),
            typeof(SwipeCardView),
            DefaultBackCardScale,
            validateValue: (_, value) => value is float f && f > 0f && f <= 1f,
            propertyChanged: OnStackVisualPropertyChanged);

    public static readonly BindableProperty CardRotationProperty =
        BindableProperty.Create(
            nameof(CardRotation),
            typeof(float),
            typeof(SwipeCardView),
            DefaultCardRotation);

    public static readonly BindableProperty AnimationLengthProperty =
        BindableProperty.Create(
            nameof(AnimationLength),
            typeof(uint),
            typeof(SwipeCardView),
            DefaultAnimationLength);

    public static readonly BindableProperty LoopCardsProperty =
        BindableProperty.Create(
            nameof(LoopCards),
            typeof(bool),
            typeof(SwipeCardView),
            DefaultLoopCards);

    public static readonly BindableProperty StackDepthProperty =
        BindableProperty.Create(
            nameof(StackDepth),
            typeof(int),
            typeof(SwipeCardView),
            0,
            validateValue: (_, value) => value is int i && i >= 0 && i <= 3,
            propertyChanged: OnStackDepthPropertyChanged);

    public static readonly BindableProperty StackOffsetProperty =
        BindableProperty.Create(
            nameof(StackOffset),
            typeof(double),
            typeof(SwipeCardView),
            10.0,
            validateValue: (_, value) => value is double d && d >= 0,
            propertyChanged: OnStackVisualPropertyChanged);

    public static readonly BindableProperty StackScaleStepProperty =
        BindableProperty.Create(
            nameof(StackScaleStep),
            typeof(double),
            typeof(SwipeCardView),
            0.03,
            validateValue: (_, value) => value is double d && d >= 0 && d <= 0.5,
            propertyChanged: OnStackVisualPropertyChanged);

    public static readonly BindableProperty StackDirectionProperty =
        BindableProperty.Create(
            nameof(StackDirection),
            typeof(StackDirection),
            typeof(SwipeCardView),
            Core.StackDirection.Bottom,
            propertyChanged: OnStackVisualPropertyChanged);

    #endregion Bindable Properties

    #region Constants

    private const uint DefaultThreshold = 100;

    private const SwipeCardDirection DefaultSupportedSwipeDirections = SwipeCardDirection.Left | SwipeCardDirection.Right | SwipeCardDirection.Up | SwipeCardDirection.Down;

    private const SwipeCardDirection DefaultSupportedDraggingDirections = SwipeCardDirection.Left | SwipeCardDirection.Right | SwipeCardDirection.Up | SwipeCardDirection.Down;

    private const float DefaultBackCardScale = 0.8f;

    private const float DefaultCardRotation = 20;

    private const uint DefaultAnimationLength = 250; // Speed of the animations

    private const bool DefaultLoopCards = false;

    private const int NumCards = 2; // Number of cards in stack

    private const uint InvokeSwipeDefaultNumberOfTouches = 20;
    private const uint InvokeSwipeDefaultTouchDifference = 10;
    private const uint InvokeSwipeDefaultTouchDelay = 1;
    private const uint InvokeSwipeDefaultEndDelay = 200;

    #endregion Constants

    #region Private fields

    private readonly View[] _cards = new View[NumCards];

    private int _topCardIndex;  // The card at the top of the stack

    private float _cardDistanceX = 0;   // Distance the card has been moved on X axis

    private float _cardDistanceY = 0;   // Distance the card has been moved on Y axis

    private int _itemIndex = 0; // The next items index to be loaded into a card
    private int _currentDisplayIndex = 0; // Index of the currently displayed top item in ItemsSource

    private bool _ignoreTouch = false;

    private bool _disposed = false;
    private bool _programmaticSwipe = false;
    private int _collectionVersion = 0;
    private INotifyCollectionChanged? _subscribedCollection;
    private readonly List<Border> _shadowCards = new();

    #endregion Private fields

    public SwipeCardView()
    {
        IsClippedToBounds = false;

        var view = new Grid { IsClippedToBounds = false };

        Content = view;

        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnPanUpdated;
        GestureRecognizers.Add(panGesture);
    }

    #region Public Properties

    /// <summary>Occurs when a card is swiped past the threshold distance.</summary>
    public event EventHandler<SwipedCardEventArgs>? Swiped;

    /// <summary>Occurs continuously while a card is being dragged, reporting direction, position, and distance.</summary>
    public event EventHandler<DraggingCardEventArgs>? Dragging;

    /// <summary>Gets or sets the data source for the card stack. Must implement <see cref="IList"/> (e.g. <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/>).</summary>
    public IList ItemsSource
    {
        get => (IList)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>Gets or sets the <see cref="DataTemplate"/> used to render each card.</summary>
    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    /// <summary>Gets the data item of the currently displayed top card (one-way-to-source).</summary>
    public object? TopItem
    {
        get => GetValue(TopItemProperty);
        set => SetValue(TopItemProperty, value);
    }

    /// <summary>Gets the data item of the previously swiped card, or null if at the first item (one-way-to-source).</summary>
    public object? PreviousItem
    {
        get => GetValue(PreviousItemProperty);
        set => SetValue(PreviousItemProperty, value);
    }

    /// <summary>Gets or sets the command executed when a card is swiped. Receives <see cref="SwipedCardEventArgs"/>.</summary>
    public ICommand SwipedCommand
    {
        get => (ICommand)GetValue(SwipedCommandProperty);
        set => SetValue(SwipedCommandProperty, value);
    }

    /// <summary>Gets or sets an optional parameter included in <see cref="SwipedCardEventArgs.Parameter"/>.</summary>
    public object SwipedCommandParameter
    {
        get => GetValue(SwipedCommandParameterProperty);
        set => SetValue(SwipedCommandParameterProperty, value);
    }

    /// <summary>Gets or sets the command executed continuously while dragging. Receives <see cref="DraggingCardEventArgs"/>.</summary>
    public ICommand DraggingCommand
    {
        get => (ICommand)GetValue(DraggingCommandProperty);
        set => SetValue(DraggingCommandProperty, value);
    }

    /// <summary>Gets or sets an optional parameter included in <see cref="DraggingCardEventArgs.Parameter"/>.</summary>
    public object DraggingCommandParameter
    {
        get => GetValue(DraggingCommandParameterProperty);
        set => SetValue(DraggingCommandParameterProperty, value);
    }

    /// <summary>Gets or sets the minimum drag distance (in device-independent units) to trigger a swipe. Default is 100.</summary>
    public uint Threshold
    {
        get => (uint)GetValue(ThresholdProperty);
        set => SetValue(ThresholdProperty, value);
    }

    /// <summary>Gets or sets which directions are allowed to complete a swipe. Default is all four.</summary>
    public SwipeCardDirection SupportedSwipeDirections
    {
        get => (SwipeCardDirection)GetValue(SupportedSwipeDirectionsProperty);
        set => SetValue(SupportedSwipeDirectionsProperty, value);
    }

    /// <summary>Gets or sets which drag directions generate drag events. Default is all four.</summary>
    public SwipeCardDirection SupportedDraggingDirections
    {
        get => (SwipeCardDirection)GetValue(SupportedDraggingDirectionsProperty);
        set => SetValue(SupportedDraggingDirectionsProperty, value);
    }

    /// <summary>Gets or sets the initial scale of the back card (0.0–1.0). Scales up to 1.0 as the top card is dragged.
    /// When <see cref="StackDepth"/> &gt; 1, this is the scale of the first back card; successive shadow cards
    /// scale down further by <see cref="StackScaleStep"/> each. Default is 0.8.</summary>
    public float BackCardScale
    {
        get => (float)GetValue(BackCardScaleProperty);
        set => SetValue(BackCardScaleProperty, value);
    }

    /// <summary>Gets or sets the maximum rotation angle (degrees) applied during horizontal drag. Default is 20.</summary>
    public float CardRotation
    {
        get => (float)GetValue(CardRotationProperty);
        set => SetValue(CardRotationProperty, value);
    }

    /// <summary>Gets or sets the duration (milliseconds) of swipe and snap-back animations. Default is 250.</summary>
    public uint AnimationLength
    {
        get => (uint)GetValue(AnimationLengthProperty);
        set => SetValue(AnimationLengthProperty, value);
    }

    /// <summary>Gets or sets whether the card stack loops back to the first item after the last card is swiped. Default is false.</summary>
    public bool LoopCards
    {
        get => (bool)GetValue(LoopCardsProperty);
        set => SetValue(LoopCardsProperty, value);
    }

    /// <summary>Gets or sets the number of decorative cards visible behind the top card to create a stack effect (0–3). Default is 0 (no stack).</summary>
    public int StackDepth
    {
        get => (int)GetValue(StackDepthProperty);
        set => SetValue(StackDepthProperty, value);
    }

    /// <summary>Gets or sets the vertical offset (in device-independent units) between each stacked card. Default is 10.</summary>
    public double StackOffset
    {
        get => (double)GetValue(StackOffsetProperty);
        set => SetValue(StackOffsetProperty, value);
    }

    /// <summary>Gets or sets the scale reduction applied to each successive card in the stack. Default is 0.03 (each card is 3% smaller).</summary>
    public double StackScaleStep
    {
        get => (double)GetValue(StackScaleStepProperty);
        set => SetValue(StackScaleStepProperty, value);
    }

    /// <summary>Gets or sets whether stacked cards peek above or below the top card. Default is Bottom.</summary>
    public StackDirection StackDirection
    {
        get => (StackDirection)GetValue(StackDirectionProperty);
        set => SetValue(StackDirectionProperty, value);
    }

    #endregion Public Properties

    #region Public Methods

    /// <summary>Releases all resources used by the <see cref="SwipeCardView"/>.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases resources used by the SwipeCardView. Override in derived classes
    /// to add custom cleanup logic (always call base.Dispose(disposing)).
    /// </summary>
    /// <param name="disposing">True if called from Dispose(), false if from finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (!disposing)
        {
            return;
        }

        foreach (var card in _cards)
        {
            if (card != null)
                Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(card);
        }

        foreach (var shadow in _shadowCards)
        {
            (shadow as IDisposable)?.Dispose();
        }
        _shadowCards.Clear();

        SizeChanged -= OnSizeChangedForStack;

        var panGesture = GestureRecognizers.OfType<PanGestureRecognizer>().FirstOrDefault();
        if (panGesture != null)
        {
            panGesture.PanUpdated -= OnPanUpdated;
        }

        GestureRecognizers.Clear();

        UnsubscribeCollectionChanged();
    }

    /// <summary>
    /// Simulates PanGesture movement to left or right
    /// </summary>
    /// <param name="swipeCardDirection">Direction of the movement. Currently supported Left and Right.</param>
    public async Task InvokeSwipe(SwipeCardDirection swipeCardDirection)
    {
        await InvokeSwipe(swipeCardDirection, InvokeSwipeDefaultNumberOfTouches,
            InvokeSwipeDefaultTouchDifference, TimeSpan.FromMilliseconds(InvokeSwipeDefaultTouchDelay),
            TimeSpan.FromMilliseconds(InvokeSwipeDefaultEndDelay));
    }

    /// <summary>
    /// Simulates PanGesture movement to left or right
    /// </summary>
    /// <param name="swipeCardDirection">Direction of the movement. Currently supported Left and Right.</param>
    /// <param name="numberOfTouches">Number of touch events. It should be a positive number (i.e. 20)</param>
    /// <param name="touchDifference">Distance passed between two touches. It should be a positive number (i.e. 10)</param>
    /// <param name="touchDelay">Delay between two touches. It should be a positive number (i.e. 1 millisecond)</param>
    /// <param name="endDelay">End delay. It should be a positive number (i.e. 200 milliseconds)</param>
    public async Task InvokeSwipe(SwipeCardDirection swipeCardDirection, uint numberOfTouches, uint touchDifference, TimeSpan touchDelay, TimeSpan endDelay)
    {
        if (_disposed || _ignoreTouch || _programmaticSwipe || numberOfTouches == 0 || touchDifference == 0)
        {
            return;
        }

        // Block user touch input during the programmatic swipe to prevent
        // concurrent gesture updates from corrupting card state.
        _programmaticSwipe = true;

        try
        {
            var differenceX = 0;
            var differenceY = 0;

            switch (swipeCardDirection)
            {
                case SwipeCardDirection.Right:
                    differenceX = (int)touchDifference;
                    break;

                case SwipeCardDirection.Left:
                    differenceX = (int)(-1 * touchDifference);
                    break;

                case SwipeCardDirection.Up:
                    differenceY = (int)(-1 * touchDifference);
                    break;

                case SwipeCardDirection.Down:
                    differenceY = (int)touchDifference;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(swipeCardDirection), swipeCardDirection, null);
            }

            HandleTouchStart();

            for (var i = 1; i < numberOfTouches; i++)
            {
                HandleTouch(differenceX * i, differenceY * i);
                await Task.Delay(touchDelay);
            }

            await Task.Delay(endDelay);

            await HandleTouchEnd();
        }
        finally
        {
            _programmaticSwipe = false;
        }
    }

    /// <summary>
    /// Goes back to the previously swiped card without animation. Returns true if successful,
    /// false if already at the first item or no items are available.
    /// </summary>
    public bool GoBack()
    {
        return GoBack(false);
    }

    /// <summary>
    /// Goes back to the previously swiped card. Returns true if successful,
    /// false if already at the first item or no items are available.
    /// </summary>
    /// <param name="animated">When true, the previous card slides in from the left with animation.</param>
    public bool GoBack(bool animated)
    {
        if (_disposed || _ignoreTouch || _programmaticSwipe)
        {
            return false;
        }

        if (ItemsSource == null || ItemsSource.Count == 0)
        {
            return false;
        }

        // If TopItem is null (all cards swiped), go back to the last item
        if (TopItem == null)
        {
            _itemIndex = ItemsSource.Count - 1;
            Setup();

            // Also reset the back card so it doesn't show stale content
            var backCard = _cards[PrevCardIndex(_topCardIndex)];
            backCard.IsVisible = false;
            backCard.Opacity = 0;

            return _itemIndex >= 0;
        }

        // Can't go back if at the first item
        if (_currentDisplayIndex <= 0)
        {
            return false;
        }

        ShowPreviousCard(animated);
        return true;
    }

    #endregion Public Methods

    #region Event Handlers

    private static void OnItemTemplatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var swipeCardView = (SwipeCardView)bindable;

        if (swipeCardView.ItemTemplate == null)
        {
            return;
        }

        swipeCardView.Content = null;

        var view = new Grid() { IsClippedToBounds = false };

        // create a stack of cards
        for (var i = NumCards - 1; i >= 0; i--)
        {
            var content = swipeCardView.ItemTemplate.CreateContent();
#pragma warning disable CS0618 // ViewCell is obsolete but still supported for backward compatibility
            if (!(content is View) && !(content is ViewCell))
            {
                throw new Exception($"Invalid visual object {nameof(content)}");
            }

            var card = content is View view1 ? view1 : ((ViewCell)content).View;
#pragma warning restore CS0618

            swipeCardView._cards[i] = card;
            card.IsVisible = false;
            Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(card);

            view.Children.Add(card);
        }

        swipeCardView.Content = view;
        swipeCardView.RebuildShadowCards();

        // If ItemsSource was already set before the template, initialize the cards now
        if (swipeCardView.ItemsSource != null && swipeCardView.ItemsSource.Count > 0)
        {
            swipeCardView._itemIndex = 0;
            swipeCardView._currentDisplayIndex = 0;
            swipeCardView.Setup();
        }
    }

    private static void OnItemsSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var swipeCardView = (SwipeCardView)bindable;
        swipeCardView.UnsubscribeCollectionChanged();

        swipeCardView._collectionVersion++;
        swipeCardView._itemIndex = 0;
        swipeCardView._currentDisplayIndex = 0;

        if (newValue == null)
        {
            // Clear all cards when ItemsSource is set to null
            foreach (var card in swipeCardView._cards)
            {
                if (card != null)
                {
                    Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(card);
                    card.IsVisible = false;
                }
            }
            swipeCardView.TopItem = null;
            swipeCardView.PreviousItem = null;
            return;
        }

        swipeCardView.Setup();
        swipeCardView.SubscribeCollectionChanged();
    }

    private void SubscribeCollectionChanged()
    {
        if (ItemsSource is INotifyCollectionChanged observable)
        {
            observable.CollectionChanged += OnItemSourceCollectionChanged;
            _subscribedCollection = observable;
        }
    }

    private void UnsubscribeCollectionChanged()
    {
        if (_subscribedCollection != null)
        {
            _subscribedCollection.CollectionChanged -= OnItemSourceCollectionChanged;
            _subscribedCollection = null;
        }
    }

    private void OnItemSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // Guard against null cards (ItemTemplate not yet set)
        if (_cards[0] == null || _cards[1] == null)
        {
            return;
        }

        _collectionVersion++;

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Reset:
                _itemIndex = 0;
                _currentDisplayIndex = 0;

                // Cancel any running animations before resetting
                foreach (var card in _cards)
                {
                    if (card != null)
                    {
                        Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(card);
                        card.IsVisible = false;
                    }
                }

                _ignoreTouch = false;
                TopItem = null;
                PreviousItem = null;
                if (ItemsSource != null && ItemsSource.Count > 0)
                    Setup();
                break;

            case NotifyCollectionChangedAction.Add:
                // If both cards are hidden (all items were swiped), show the new items
                if (_cards[0].IsVisible == false && _cards[1].IsVisible == false)
                {
                    _itemIndex = e.NewStartingIndex;
                    _currentDisplayIndex = e.NewStartingIndex;
                    Setup();
                }
                else if (e.NewStartingIndex <= _currentDisplayIndex)
                {
                    // Items inserted before/at current position shift indices
                    var insertCount = e.NewItems?.Count ?? 1;
                    _currentDisplayIndex += insertCount;
                    _itemIndex += insertCount;
                }

                // Prepare the back card if it hasn't been initialized yet.
                // This happens when Setup() ran with only 1 item available (e.g.,
                // after Clear + Add, or exhausting the stack then adding new items).
                var addBackIdx = NextCardIndex(_topCardIndex);
                if (_cards[addBackIdx] != null && !_cards[addBackIdx].IsVisible && _itemIndex < ItemsSource.Count)
                {
                    var backCard = _cards[addBackIdx];
                    backCard.BindingContext = ItemsSource[_itemIndex];
                    backCard.Scale = 1.0;
                    backCard.Rotation = 0;
                    backCard.TranslationX = 0;
                    backCard.TranslationY = -backCard.Y;
                    backCard.ZIndex = 0;
                    backCard.Opacity = 0;
                    backCard.IsVisible = true;
                    _itemIndex++;
                }
                break;

            case NotifyCollectionChangedAction.Remove:
                if (ItemsSource == null || ItemsSource.Count == 0)
                {
                    // All items removed — hide cards
                    foreach (var card in _cards)
                    {
                        if (card != null)
                        {
                            Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(card);
                            card.IsVisible = false;
                        }
                    }
                    _itemIndex = 0;
                    _currentDisplayIndex = 0;
                    TopItem = null;
                    PreviousItem = null;
                }
                else if (e.OldStartingIndex == _currentDisplayIndex)
                {
                    // The currently displayed item was removed — reinitialize at same position
                    _currentDisplayIndex = Math.Min(_currentDisplayIndex, ItemsSource.Count - 1);
                    _itemIndex = _currentDisplayIndex;
                    Setup();
                }
                else if (e.OldStartingIndex < _currentDisplayIndex)
                {
                    // An item before the current display was removed — shift indices down
                    var removeCount = e.OldItems?.Count ?? 1;
                    _currentDisplayIndex = Math.Max(0, _currentDisplayIndex - removeCount);
                    _itemIndex = Math.Max(0, _itemIndex - removeCount);
                }
                else if (e.OldStartingIndex > _currentDisplayIndex)
                {
                    // An item after current was removed — update _itemIndex and rebind back card if needed
                    _itemIndex = Math.Min(_itemIndex, ItemsSource.Count);
                    var backIdx = NextCardIndex(_topCardIndex);
                    var backCard = _cards[backIdx];
                    if (backCard.IsVisible && _currentDisplayIndex + 1 < ItemsSource.Count)
                    {
                        backCard.BindingContext = ItemsSource[_currentDisplayIndex + 1];
                    }
                    else if (backCard.IsVisible)
                    {
                        backCard.IsVisible = false;
                        backCard.Opacity = 0;
                    }
                }
                break;

            case NotifyCollectionChangedAction.Replace:
                if (ItemsSource == null) break;
                // If the replaced item is currently displayed, update its binding
                if (e.NewStartingIndex == _currentDisplayIndex && _currentDisplayIndex < ItemsSource.Count)
                {
                    var topCard = _cards[_topCardIndex];
                    topCard.BindingContext = ItemsSource[_currentDisplayIndex];
                    TopItem = topCard.BindingContext;
                }
                else if (e.NewStartingIndex == _currentDisplayIndex + 1 && _currentDisplayIndex + 1 < ItemsSource.Count)
                {
                    // The back card item was replaced — update its binding if visible
                    var backCard = _cards[NextCardIndex(_topCardIndex)];
                    if (backCard.IsVisible || backCard.Opacity > 0)
                    {
                        backCard.BindingContext = ItemsSource[_currentDisplayIndex + 1];
                    }
                }
                break;

            case NotifyCollectionChangedAction.Move:
                // Reinitialize — move operations change the order unpredictably
                _itemIndex = 0;
                _currentDisplayIndex = 0;
                Setup();
                break;
        }
    }

    private async void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (_disposed || _programmaticSwipe || ItemsSource == null || ItemsSource.Count == 0)
        {
            return;
        }

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                HandleTouchStart();
                break;

            case GestureStatus.Running:
                HandleTouch((float)e.TotalX, (float)e.TotalY);
                break;

            case GestureStatus.Completed:
                await HandleTouchEnd();
                break;

            case GestureStatus.Canceled:
                await HandleTouchEnd();
                break;
        }
    }

    #endregion Event Handlers

    #region Private Methods

    private static void OnStackDepthPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var swipeCardView = (SwipeCardView)bindable;
        swipeCardView.RebuildShadowCards();
        swipeCardView.ApplyStackVisuals();
        swipeCardView.PositionShadowCards();
    }

    private static void OnStackVisualPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var swipeCardView = (SwipeCardView)bindable;
        swipeCardView.ApplyStackVisuals();
        swipeCardView.PositionShadowCards();
    }

    /// <summary>Creates or removes decorative Border elements for the stack visual effect.</summary>
    private void RebuildShadowCards()
    {
        if (Content is not Grid grid) return;

        // Remove existing shadow cards
        foreach (var shadow in _shadowCards)
        {
            grid.Children.Remove(shadow);
            (shadow as IDisposable)?.Dispose();
        }
        _shadowCards.Clear();

        if (StackDepth <= 1) return;

        // The back card provides level 1 of the stack. We create (StackDepth - 1)
        // additional shadow cards for levels 2+.
        int shadowCount = StackDepth - 1;
        for (int i = 0; i < shadowCount; i++)
        {
            var shadow = new Border
            {
                BackgroundColor = Colors.White,
                Stroke = new SolidColorBrush(Color.FromArgb("#E0E0E0")),
                StrokeThickness = 1,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(10) },
                ZIndex = -1 - i,
                InputTransparent = true,
                IsVisible = false,
                AnchorY = 0.5,
                Opacity = Math.Max(0.3, 1.0 - (0.2 * (i + 1)))
            };
            _shadowCards.Add(shadow);
            grid.Children.Insert(0, shadow);
        }
    }

    /// <summary>Positions shadow cards at their rest state behind the real cards.</summary>
    private void PositionShadowCards()
    {
        if (_shadowCards.Count == 0 || Content is not Grid) return;

        var hasItems = ItemsSource != null && ItemsSource.Count > 0;
        // Use back card height for compensation; fall back to container height
        var refHeight = _cards[PrevCardIndex(_topCardIndex)]?.Height ?? Height;
        if (refHeight <= 0) refHeight = Height;

        var dirSign = StackDirection == Core.StackDirection.Top ? -1.0 : 1.0;

        for (int i = 0; i < _shadowCards.Count; i++)
        {
            var shadow = _shadowCards[i];
            // Shadow i sits at depth (i + 2): back card is depth 1.
            var scale = Math.Max(0.5, BackCardScale - ((i + 1) * StackScaleStep));
            var compensation = refHeight * (1.0 - scale) / 2.0;
            shadow.AnchorY = 0.5;
            shadow.Scale = scale;
            shadow.TranslationY = dirSign * ((i + 2) * StackOffset + compensation);
            shadow.IsVisible = hasItems;
        }
    }

    /// <summary>Animates shadow cards during a swipe drag based on progress ratio (0..1).</summary>
    private void AnimateShadowCards(float progressRatio)
    {
        if (_shadowCards.Count == 0) return;

        progressRatio = Math.Min(Math.Max(progressRatio, 0f), 1f);

        var refHeight = _cards[PrevCardIndex(_topCardIndex)]?.Height ?? Height;
        if (refHeight <= 0) refHeight = Height;

        var dirSign = StackDirection == Core.StackDirection.Top ? -1.0 : 1.0;

        for (int i = 0; i < _shadowCards.Count; i++)
        {
            // Shadow i animates from depth (i+2) toward depth (i+1).
            var restScale = Math.Max(0.5, BackCardScale - ((i + 1) * StackScaleStep));
            var targetScale = Math.Max(0.5, restScale + (progressRatio * StackScaleStep));
            var compensation = refHeight * (1.0 - targetScale) / 2.0;
            var restTY = (i + 2) * StackOffset;
            var targetTY = restTY - (progressRatio * StackOffset);

            _shadowCards[i].Scale = targetScale;
            _shadowCards[i].TranslationY = dirSign * (targetTY + compensation);
        }
    }

    /// <summary>Resets shadow cards to rest position after a swipe or snap-back.</summary>
    private void ResetShadowCards()
    {
        PositionShadowCards();
    }

    private void Setup()
    {
        if (ItemsSource == null)
        {
            return;
        }

        // Guard: cards are not yet created (ItemTemplate not set)
        if (_cards[0] == null || _cards[1] == null || Content == null)
        {
            return;
        }

        // Set the top card
        _topCardIndex = 0;

        // Create a stack of cards
        var wasVisible = Content.IsVisible;
        Content.IsVisible = false;
        for (var i = 0; i < Math.Min(NumCards, ItemsSource.Count); i++)
        {
            if (_itemIndex >= ItemsSource.Count)
            {
                if (LoopCards)
                    _itemIndex = 0;
                else
                    break;
            }

            var card = _cards[i];
            card.BindingContext = ItemsSource[_itemIndex];

            if (i == 0)
            {
                TopItem = ItemsSource[_itemIndex];
                _currentDisplayIndex = _itemIndex;
            }

            Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(card);
            // Always lay out cards at Scale=1.0 so the native platform computes
            // full-size bounds. The BackCardScale is applied only as a temporary
            // visual transform when dragging starts (see HandleTouchStart).
            card.Scale = 1.0;
            card.AnchorY = 0.5;
            card.Rotation = 0;
            card.TranslationX = 0;
            card.TranslationY = -card.Y;
            // Use fixed ZIndex: top card = 1, back card = 0
            card.ZIndex = (i == _topCardIndex) ? 1 : 0;
            // Hide back card to prevent rendering bleed-through;
            // it becomes visible when dragging starts (HandleTouchStart)
            card.Opacity = (i == _topCardIndex) ? 1 : 0;
            card.IsVisible = true;
            _itemIndex++;
        }
        Content.IsVisible = wasVisible;

        // When stack effect is enabled, show the back card and shadow cards
        // after layout has computed bounds at Scale=1.0.
        if (StackDepth > 0)
        {
            try
            {
                Dispatcher.Dispatch(() => ApplyStackVisuals());
            }
            catch (InvalidOperationException)
            {
                // No dispatcher (unit test context) — apply synchronously
                ApplyStackVisuals();
            }
        }

        PositionShadowCards();
    }

    /// <summary>Computes the TranslationY needed for a back card to peek by the desired offset.</summary>
    /// <remarks>
    /// With center-anchored scaling (AnchorY=0.5), a scaled card's edges retract by H*(1-Scale)/2.
    /// For Bottom direction, we compensate the bottom retraction so the card peeks below.
    /// For Top direction, we compensate the top retraction so the card peeks above.
    /// </remarks>
    private double ComputeStackTranslationY(View card, double scale, double peekAmount)
    {
        var height = card.Height;
        if (height <= 0) height = Height; // fallback to container height
        var compensation = height * (1.0 - scale) / 2.0;

        if (StackDirection == Core.StackDirection.Top)
        {
            // Peek above: negative offset, subtract compensation (top edge retracts inward)
            return -card.Y - peekAmount - compensation;
        }

        // Peek below: positive offset, add compensation (bottom edge retracts inward)
        return -card.Y + peekAmount + compensation;
    }

    /// <summary>Applies visual transforms for the stack effect (called after layout pass).</summary>
    private void ApplyStackVisuals()
    {
        if (StackDepth <= 0 || ItemsSource == null || ItemsSource.Count < 2) return;

        var backCard = _cards[PrevCardIndex(_topCardIndex)];
        if (backCard == null || !backCard.IsVisible) return;

        // We need the card height (or container height) for compensation.
        // If neither is available yet (first layout pass), subscribe to SizeChanged.
        var refHeight = backCard.Height > 0 ? backCard.Height : Height;
        if (refHeight <= 0)
        {
            // Heights not ready — defer until layout completes
            SizeChanged -= OnSizeChangedForStack;
            SizeChanged += OnSizeChangedForStack;
            return;
        }

        // Center-anchored scaling with compensated TranslationY:
        // The card is scaled from its center (AnchorY=0.5), then pushed down enough
        // that its bottom edge peeks exactly StackOffset dp below the top card.
        backCard.AnchorY = 0.5;
        backCard.Scale = BackCardScale;
        backCard.Opacity = 1;
        backCard.TranslationY = ComputeStackTranslationY(backCard, BackCardScale, StackOffset);
    }

    private void OnSizeChangedForStack(object? sender, EventArgs e)
    {
        if (Height > 0)
        {
            SizeChanged -= OnSizeChangedForStack;
            ApplyStackVisuals();
            PositionShadowCards();
        }
    }

    // Handle when a touch event begins
    private void HandleTouchStart()
    {
        if (_ignoreTouch)
        {
            return;
        }

        var topCard = _cards[_topCardIndex];
        if (topCard == null)
        {
            return;
        }

        // Reveal the back card for the parallax scale effect during drag.
        // Apply BackCardScale now (not during layout) so the native platform
        // always computes full-size bounds for the card.
        var backCard = _cards[PrevCardIndex(_topCardIndex)];
        if (backCard != null)
        {
            backCard.Scale = BackCardScale;
            backCard.Opacity = 1;
            // When stack is active, move back card from offset position to top
            if (StackDepth > 0)
            {
                backCard.TranslationY = -backCard.Y;
            }
        }

        _cardDistanceX = 0;
        _cardDistanceY = 0;
        SendDragging(topCard, SwipeCardDirection.None, DraggingCardPosition.Start, 0, 0);
    }

    // Handle the ongoing touch event as the card is moved
    private void HandleTouch(float differenceX, float differenceY)
    {
        if (_ignoreTouch)
        {
            return;
        }

        var topCard = _cards[_topCardIndex];
        var backCard = _cards[PrevCardIndex(_topCardIndex)];

        // Move the top card
        if (topCard != null && topCard.IsVisible)
        {
            // Move the card
            if ((differenceX > 0 && SupportedDraggingDirections.IsRight()) || (differenceX < 0 && SupportedDraggingDirections.IsLeft()))
            {
                topCard.TranslationX = differenceX;

                // Calculate a angle for the card (guard against Width==0 before layout)
                if (Width > 0)
                {
                    var rotationAngle = (float)(CardRotation * Math.Min(differenceX / Width, 1.0f));
                    topCard.Rotation = rotationAngle;
                }
            }

            if ((differenceY > 0 && SupportedDraggingDirections.IsDown()) || (differenceY < 0 && SupportedDraggingDirections.IsUp()))
            {
                topCard.TranslationY = differenceY;
            }

            // Keep a record of how far its moved
            _cardDistanceX = differenceX;
            _cardDistanceY = differenceY;

            // Only send dragging events when there's actual movement
            if (Math.Abs(differenceX) > 0 || Math.Abs(differenceY) > 0)
            {
                SwipeCardDirection direction;
                DraggingCardPosition position;
                if (Math.Abs(differenceX) > Math.Abs(differenceY))
                {
                    direction = differenceX > 0 ? SwipeCardDirection.Right : SwipeCardDirection.Left;
                    position = Math.Abs(differenceX) > Threshold ? DraggingCardPosition.OverThreshold : DraggingCardPosition.UnderThreshold;
                }
                else
                {
                    direction = differenceY > 0 ? SwipeCardDirection.Down : SwipeCardDirection.Up;
                    position = Math.Abs(differenceY) > Threshold ? DraggingCardPosition.OverThreshold : DraggingCardPosition.UnderThreshold;
                }

                if (SupportedDraggingDirections.IsSupported(direction))
                {
                    SendDragging(topCard, direction, position, differenceX, differenceY);
                }
            }
        }

        // Scale the back card (guard against Threshold==0)
        if (backCard != null && backCard.IsVisible && Threshold > 0)
        {
            var cardDistance = Math.Abs(differenceX) > Math.Abs(differenceY) ? differenceX : differenceY;
            var newScale = Math.Min(BackCardScale + Math.Abs((cardDistance / Threshold) * (1.0f - BackCardScale)), 1.0f);
            backCard.Scale = newScale;

            // When stack is active, update TranslationY to track the changing scale
            // (compensation depends on scale, so it must update each frame)
            if (StackDepth > 0)
            {
                var progressRatio = (float)Math.Min(Math.Abs(cardDistance) / Threshold, 1.0);
                var targetPeek = StackOffset * (1.0 - progressRatio); // peek shrinks as card grows
                backCard.TranslationY = ComputeStackTranslationY(backCard, newScale, targetPeek);
            }

            // Animate shadow cards in sync with the back card scale
            if (_shadowCards.Count > 0)
            {
                var progressRatio = (float)Math.Min(Math.Abs(cardDistance) / Threshold, 1.0);
                AnimateShadowCards(progressRatio);
            }
        }
    }

    // Handle the end of the touch event
    private async Task HandleTouchEnd()
    {
        if (_ignoreTouch)
        {
            return;
        }

        _ignoreTouch = true;

        var topCard = _cards[_topCardIndex];
        if (topCard == null)
        {
            _ignoreTouch = false;
            return;
        }

        try
        {
            SwipeCardDirection direction;
            DraggingCardPosition position;
            if (Math.Abs(_cardDistanceX) > Math.Abs(_cardDistanceY))
            {
                direction = _cardDistanceX > 0 ? SwipeCardDirection.Right : SwipeCardDirection.Left;
                position = Math.Abs(_cardDistanceX) > Threshold ? DraggingCardPosition.FinishedOverThreshold : DraggingCardPosition.FinishedUnderThreshold;
            }
            else
            {
                direction = _cardDistanceY > 0 ? SwipeCardDirection.Down : SwipeCardDirection.Up;
                position = Math.Abs(_cardDistanceY) > Threshold ? DraggingCardPosition.FinishedOverThreshold : DraggingCardPosition.FinishedUnderThreshold;
            }

            if (SupportedDraggingDirections.IsSupported(direction))
            {
                SendDragging(topCard, direction, position, _cardDistanceX, _cardDistanceY);
            }

            if (position == DraggingCardPosition.FinishedOverThreshold && SupportedSwipeDirections.IsSupported(direction))
            {
                // Animate the top card off the screen (best-effort)
                try
                {
                    if (direction.IsLeft() || direction.IsRight())
                    {
                        await topCard.TranslateToAsync(_cardDistanceX > 0 ? Width : -Width, 0, AnimationLength, Easing.CubicIn);
                    }
                    else
                    {
                        await topCard.TranslateToAsync(0, _cardDistanceY > 0 ? Height : -Height, AnimationLength, Easing.CubicIn);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"HandleTouchEnd swipe animation error: {ex.Message}");
                }

                // Cancel any lingering scale animation on the back card and ensure
                // it's at full scale before it becomes the new top card
                var backCard = _cards[NextCardIndex(_topCardIndex)];
                backCard.CancelAnimations();
                backCard.Scale = 1.0;
                backCard.AnchorY = 0.5;
                InvalidateCardLayout(backCard);

                // State transition must happen regardless of animation success
                topCard.IsVisible = false;
                topCard.Scale = 0.0;
                topCard.Rotation = 0;
                topCard.TranslationX = 0;
                topCard.TranslationY = -topCard.Y;

                var collectionVersionBeforeSwipe = _collectionVersion;
                SendSwiped(topCard, direction);

                // Only advance to next card if the collection wasn't modified by the Swiped handler.
                // If a handler removed the swiped item (common pattern), OnItemSourceCollectionChanged
                // already updated internal state — calling ShowNextCard would double-advance.
                if (_collectionVersion == collectionVersionBeforeSwipe)
                {
                    ShowNextCard();
                }
            }
            else
            {
                var prevCard = _cards[PrevCardIndex(_topCardIndex)];
                prevCard.CancelAnimations();

                // Snap-back animations (best-effort)
                try
                {
                    topCard.CancelAnimations();

                    // Animate top card back to home position
                    var translateTask = topCard.TranslateToAsync(0, -topCard.Y, AnimationLength, Easing.SpringOut);
                    var rotateTask = topCard.RotateToAsync(0, AnimationLength, Easing.SpringOut);
                    // Animate back card scale down (the nice parallax shrink effect)
                    var scaleTask = prevCard.ScaleToAsync(BackCardScale, AnimationLength, Easing.SpringOut);

                    await Task.WhenAll(translateTask, rotateTask, scaleTask);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"HandleTouchEnd snap-back animation error: {ex.Message}");
                }

                // Ensure top card is at its correct home state after snap-back
                topCard.Scale = 1.0;
                topCard.TranslationX = 0;

                // After the visible animation completes, reset the back card.
                if (StackDepth > 0)
                {
                    // Stack mode: keep back card visible at offset position
                    prevCard.Opacity = 1;
                    prevCard.AnchorY = 0.5;
                    prevCard.Scale = BackCardScale;
                    prevCard.TranslationY = ComputeStackTranslationY(prevCard, BackCardScale, StackOffset);
                    ResetShadowCards();
                }
                else
                {
                    // Normal mode: hide the back card and reset scale to 1.0
                    // to prevent Android layout cache issues.
                    prevCard.Opacity = 0;
                    prevCard.Scale = 1.0;
                    InvalidateCardLayout(prevCard);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"HandleTouchEnd error: {ex}");
        }
        finally
        {
            _ignoreTouch = false;
        }
    }

    private void ShowNextCard()
    {
        if (_cards[0].IsVisible == false && _cards[1].IsVisible == false)
        {
            TopItem = null;
            Setup();
            return;
        }

        // Update _topCardIndex to point to the new top card
        var oldTopCard = _cards[_topCardIndex];
        _topCardIndex = NextCardIndex(_topCardIndex);

        // Set TopItem to the binding context of the new top card
        TopItem = _cards[_topCardIndex]?.BindingContext;
        _currentDisplayIndex++;

        // Keep _currentDisplayIndex in range when looping
        if (LoopCards && ItemsSource != null && ItemsSource.Count > 0)
        {
            _currentDisplayIndex = _currentDisplayIndex % ItemsSource.Count;
        }

        // Track the previous item for GoBack support
        PreviousItem = oldTopCard?.BindingContext;

        // Batch all property changes so the platform renderer applies them
        // in a single pass, avoiding partial-update rendering glitches.
        var newTopCard = _cards[_topCardIndex];
        newTopCard.BatchBegin();
        newTopCard.ZIndex = 1;
        newTopCard.Scale = 1.0;
        newTopCard.AnchorY = 0.5;
        newTopCard.TranslationX = 0;
        newTopCard.TranslationY = -newTopCard.Y;
        newTopCard.Opacity = 1;
        newTopCard.BatchCommit();

        // Force layout invalidation to clear any stale Android layout cache
        // from when the card was at BackCardScale during the drag.
        InvalidateCardLayout(newTopCard);

        // If there are more cards to show, recycle the old top card
        // for the next item in the source
        if (ItemsSource is not null && _itemIndex >= ItemsSource.Count && LoopCards)
        {
            _itemIndex = 0;
        }

        if (ItemsSource is not null && _itemIndex < ItemsSource.Count)
        {
            // Cancel any lingering animations before recycling
            if (oldTopCard == null) return;
            oldTopCard.CancelAnimations();

            // Push it behind the top card
            oldTopCard.ZIndex = 0;

            // Reset to full scale so the native platform computes correct bounds.
            // BackCardScale is applied later when dragging starts (HandleTouchStart).
            oldTopCard.Scale = 1.0;
            oldTopCard.Rotation = 0;
            oldTopCard.TranslationX = 0;
            oldTopCard.TranslationY = -oldTopCard.Y;

            oldTopCard.BindingContext = ItemsSource[_itemIndex];
            oldTopCard.IsVisible = true;

            if (StackDepth > 0)
            {
                // Stack mode: show back card at offset position
                oldTopCard.AnchorY = 0.5;
                oldTopCard.Opacity = 1;
                oldTopCard.Scale = BackCardScale;
                oldTopCard.TranslationY = ComputeStackTranslationY(oldTopCard, BackCardScale, StackOffset);
            }
            else
            {
                // Normal mode: hide recycled back card to prevent rendering bleed-through
                oldTopCard.AnchorY = 0.5;
                oldTopCard.Opacity = 0;
            }
            _itemIndex++;
        }

        ResetShadowCards();
    }

    private async void ShowPreviousCard(bool animated = false)
    {
        try
        {
            if (ItemsSource == null || ItemsSource.Count == 0)
            {
                return;
            }

        // Use _currentDisplayIndex for O(1) index resolution (handles duplicates correctly)
        var previousItem = ItemsSource[_currentDisplayIndex - 1];

        // The back card will become the new top card showing the previous item.
        // The current top card becomes the new back card.
        var oldTopCard = _cards[_topCardIndex];
        var newTopCardIndex = PrevCardIndex(_topCardIndex);
        var newTopCard = _cards[newTopCardIndex];

        // Cancel any animations
        oldTopCard.CancelAnimations();
        newTopCard.CancelAnimations();

        // Push the old top card behind BEFORE positioning the new card.
        // Both cards must never share ZIndex=1, otherwise visual tree order
        // determines rendering and the new card can end up behind the old one.
        oldTopCard.ZIndex = 0;

        if (animated)
        {
            // Block touch input during animation
            _ignoreTouch = true;

            try
            {
                // Position the new top card off-screen to the left, ready to slide in
                newTopCard.BatchBegin();
                newTopCard.BindingContext = previousItem;
                newTopCard.Scale = 1.0;
                newTopCard.AnchorY = 0.5;
                newTopCard.Rotation = 0;
                newTopCard.TranslationX = -Width;
                newTopCard.TranslationY = -newTopCard.Y;
                newTopCard.ZIndex = 1;
                newTopCard.Opacity = 1;
                newTopCard.IsVisible = true;
                newTopCard.BatchCommit();

                // Slide the new card in from the left.
                // TranslateToAsync may throw in unit test context (no IAnimationManager),
                // so fall through to the final state if animation fails.
                try
                {
                    await newTopCard.TranslateToAsync(0, -newTopCard.Y, AnimationLength, Easing.CubicOut);
                }
                catch (ArgumentException)
                {
                    newTopCard.TranslationX = 0;
                }

                // Position the old top card as the back card
                oldTopCard.BatchBegin();
                oldTopCard.AnchorY = 0.5;
                oldTopCard.Scale = StackDepth > 0 ? (double)BackCardScale : 1.0;
                oldTopCard.Opacity = StackDepth > 0 ? 1 : 0;
                if (StackDepth > 0)
                    oldTopCard.TranslationY = ComputeStackTranslationY(oldTopCard, BackCardScale, StackOffset);
                oldTopCard.BatchCommit();
            }
            finally
            {
                _ignoreTouch = false;
            }
        }
        else
        {
            // Instant swap (no animation)
            newTopCard.BatchBegin();
            newTopCard.BindingContext = previousItem;
            newTopCard.Scale = 1.0;
            newTopCard.AnchorY = 0.5;
            newTopCard.Rotation = 0;
            newTopCard.TranslationX = 0;
            newTopCard.TranslationY = -newTopCard.Y;
            newTopCard.ZIndex = 1;
            newTopCard.Opacity = 1;
            newTopCard.IsVisible = true;
            newTopCard.BatchCommit();

            // Position the old top card as the back card
            oldTopCard.BatchBegin();
            oldTopCard.AnchorY = 0.5;
            oldTopCard.Scale = StackDepth > 0 ? (double)BackCardScale : 1.0;
            oldTopCard.Opacity = StackDepth > 0 ? 1 : 0;
            if (StackDepth > 0)
                oldTopCard.TranslationY = ComputeStackTranslationY(oldTopCard, BackCardScale, StackOffset);
            oldTopCard.BatchCommit();
        }

        // Update indices
        _currentDisplayIndex--;
        _topCardIndex = newTopCardIndex;
        // _itemIndex must skip past the item already loaded on the back card
        // (the old top card retains ItemsSource[_currentDisplayIndex + 1]).
        // Using +1 here caused the recycled card to duplicate the top card's content.
        _itemIndex = _currentDisplayIndex + 2;
        TopItem = previousItem;
        PreviousItem = _currentDisplayIndex >= 1 ? ItemsSource[_currentDisplayIndex - 1] : null;

        ResetShadowCards();

        // Force layout recalculation
        try
        {
            Dispatcher.Dispatch(() =>
            {
                newTopCard.InvalidateMeasure();
                ((View)newTopCard.Parent)?.InvalidateMeasure();
            });
        }
        catch (InvalidOperationException)
        {
            // No dispatcher available (e.g. unit test context) — skip layout invalidation
        }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ShowPreviousCard error: {ex}");
        }
    }

    // Return the next card index from the top
    private int NextCardIndex(int topIndex)
    {
        return topIndex == 0 ? 1 : 0;
    }

    // Return the prev card index from the top
    private int PrevCardIndex(int topIndex)
    {
        return topIndex == 0 ? 1 : 0;
    }

    // Helper to get the scale based on the card index position relative to the top card
    private float GetScale(int index)
    {
        return index == _topCardIndex ? 1.0f : BackCardScale;
    }

    /// <summary>
    /// Forces a layout invalidation on a card to clear any stale Android layout cache.
    /// Android's native CardView can cache layout bounds computed while the card was
    /// at a non-1.0 Scale (e.g. BackCardScale=0.8 during drag). This method ensures
    /// Android re-measures the card at its current Scale.
    /// </summary>
    private void InvalidateCardLayout(View card)
    {
        try
        {
            card.InvalidateMeasure();
            ((View?)card.Parent)?.InvalidateMeasure();
        }
        catch (Exception)
        {
            // Ignore — may fail in unit test context without a renderer
        }
    }

    private void SendSwiped(View sender, SwipeCardDirection direction)
    {
        var args = new SwipedCardEventArgs(sender.BindingContext, SwipedCommandParameter, direction);

        var cmd = SwipedCommand;
        if (cmd != null && cmd.CanExecute(args))
        {
            cmd.Execute(args);
        }

        Swiped?.Invoke(this, args);
    }

    private void SendDragging(View sender, SwipeCardDirection direction, DraggingCardPosition position, double distanceDraggedX, double distanceDraggedY)
    {
        var args = new DraggingCardEventArgs(sender.BindingContext, DraggingCommandParameter, direction, position, distanceDraggedX, distanceDraggedY, sender);

        var cmd = DraggingCommand;
        if (cmd != null && cmd.CanExecute(args))
        {
            cmd.Execute(args);
        }

        Dragging?.Invoke(this, args);
    }

    #endregion Private Methods
}