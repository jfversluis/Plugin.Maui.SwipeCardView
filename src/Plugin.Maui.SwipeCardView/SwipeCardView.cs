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
            DefaultBackCardScale);

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

    #endregion Private fields

    public SwipeCardView()
    {
        var view = new Grid();

        Content = view;

        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnPanUpdated;
        GestureRecognizers.Add(panGesture);
    }

    #region Public Properties

    public event EventHandler<SwipedCardEventArgs>? Swiped;

    public event EventHandler<DraggingCardEventArgs>? Dragging;

    public IList ItemsSource
    {
        get => (IList)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public object? TopItem
    {
        get => GetValue(TopItemProperty);
        set => SetValue(TopItemProperty, value);
    }

    public object? PreviousItem
    {
        get => GetValue(PreviousItemProperty);
        set => SetValue(PreviousItemProperty, value);
    }

    public ICommand SwipedCommand
    {
        get => (ICommand)GetValue(SwipedCommandProperty);
        set => SetValue(SwipedCommandProperty, value);
    }

    public object SwipedCommandParameter
    {
        get => GetValue(SwipedCommandParameterProperty);
        set => SetValue(SwipedCommandParameterProperty, value);
    }

    public ICommand DraggingCommand
    {
        get => (ICommand)GetValue(DraggingCommandProperty);
        set => SetValue(DraggingCommandProperty, value);
    }

    public object DraggingCommandParameter
    {
        get => GetValue(DraggingCommandParameterProperty);
        set => SetValue(DraggingCommandParameterProperty, value);
    }

    public uint Threshold
    {
        get => (uint)GetValue(ThresholdProperty);
        set => SetValue(ThresholdProperty, value);
    }

    public SwipeCardDirection SupportedSwipeDirections
    {
        get => (SwipeCardDirection)GetValue(SupportedSwipeDirectionsProperty);
        set => SetValue(SupportedSwipeDirectionsProperty, value);
    }

    public SwipeCardDirection SupportedDraggingDirections
    {
        get => (SwipeCardDirection)GetValue(SupportedDraggingDirectionsProperty);
        set => SetValue(SupportedDraggingDirectionsProperty, value);
    }

    public float BackCardScale
    {
        get => (float)GetValue(BackCardScaleProperty);
        set => SetValue(BackCardScaleProperty, value);
    }

    public float CardRotation
    {
        get => (float)GetValue(CardRotationProperty);
        set => SetValue(CardRotationProperty, value);
    }

    public uint AnimationLength
    {
        get => (uint)GetValue(AnimationLengthProperty);
        set => SetValue(AnimationLengthProperty, value);
    }

    public bool LoopCards
    {
        get => (bool)GetValue(LoopCardsProperty);
        set => SetValue(LoopCardsProperty, value);
    }

    #endregion Public Properties

    #region Public Methods

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        foreach (var card in _cards)
        {
            if (card != null)
                Microsoft.Maui.Controls.ViewExtensions.CancelAnimations(card);
        }

        var panGesture = GestureRecognizers.OfType<PanGestureRecognizer>().FirstOrDefault();
        if (panGesture != null)
        {
            panGesture.PanUpdated -= OnPanUpdated;
        }

        GestureRecognizers.Clear();

        if (ItemsSource is INotifyCollectionChanged observable)
        {
            observable.CollectionChanged -= OnItemSourceCollectionChanged;
        }
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

    #endregion Public Methods

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

    #region Event Handlers

    private static void OnItemTemplatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var swipeCardView = (SwipeCardView)bindable;

        if (swipeCardView.ItemTemplate == null)
        {
            return;
        }

        swipeCardView.Content = null;

        var view = new Grid();

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
        var observable = oldValue as INotifyCollectionChanged;
        if (observable != null)
        {
            observable.CollectionChanged -= swipeCardView.OnItemSourceCollectionChanged;
        }

        observable = newValue as INotifyCollectionChanged;
        swipeCardView._itemIndex = 0;
        swipeCardView._currentDisplayIndex = 0;
        swipeCardView.Setup();
        if (observable != null)
        {
            observable.CollectionChanged += swipeCardView.OnItemSourceCollectionChanged;
        }
    }

    private void OnItemSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
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
            if (ItemsSource.Count > 0)
                Setup();

            return;
        }

        // Guard against null cards (ItemTemplate not yet set)
        if (_cards[0] == null || _cards[1] == null)
        {
            return;
        }

        if (_cards[0].IsVisible == false && _cards[1].IsVisible == false)
        {
            Setup();
        }
    }

    private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
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
                _ = HandleTouchEnd();
                break;

            case GestureStatus.Canceled:
                _ = HandleTouchEnd();
                break;
        }
    }

    #endregion Event Handlers

    #region Private Methods

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
            backCard.Scale = Math.Min(BackCardScale + Math.Abs((cardDistance / Threshold) * (1.0f - BackCardScale)), 1.0f);
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

                // State transition must happen regardless of animation success
                topCard.IsVisible = false;
                topCard.Scale = 0.0;
                topCard.Rotation = 0;
                topCard.TranslationX = 0;
                topCard.TranslationY = -topCard.Y;

                SendSwiped(topCard, direction);
                ShowNextCard();
            }
            else
            {
                // Snap-back animations (best-effort)
                try
                {
                    // Cancel any lingering animations before starting snap-back
                    topCard.CancelAnimations();
                    var prevCard = _cards[PrevCardIndex(_topCardIndex)];
                    prevCard.CancelAnimations();

                    // Snap-back: TranslationX must animate to 0 (matching Setup home position)
                    var translateTask = topCard.TranslateToAsync(0, -topCard.Y, AnimationLength, Easing.SpringOut);
                    var rotateTask = topCard.RotateToAsync(0, AnimationLength, Easing.SpringOut);
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

                // Hide back card again and reset Scale to 1.0 for correct layout
                var snapBackCard = _cards[PrevCardIndex(_topCardIndex)];
                snapBackCard.Opacity = 0;
                snapBackCard.Scale = 1.0;
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

        // Track the previous item for GoBack support
        PreviousItem = oldTopCard?.BindingContext;

        // Batch all property changes so the platform renderer applies them
        // in a single pass, avoiding partial-update rendering glitches.
        var newTopCard = _cards[_topCardIndex];
        newTopCard.BatchBegin();
        newTopCard.ZIndex = 1;
        newTopCard.Scale = 1.0;
        newTopCard.TranslationX = 0;
        newTopCard.Opacity = 1;
        newTopCard.BatchCommit();

        // Schedule a deferred layout invalidation after the current layout cycle.
        // This forces Android to re-measure the card at Scale=1.0, which is needed
        // when inner content was modified during the drag (e.g. label text changes)
        // while the card was at BackCardScale, causing the CardView to cache stale bounds.
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

        // If there are more cards to show, recycle the old top card
        // for the next item in the source
        if (_itemIndex < ItemsSource.Count)
        {
            // Cancel any lingering animations before recycling
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
            // Hide recycled back card to prevent rendering bleed-through
            oldTopCard.Opacity = 0;
            _itemIndex++;
        }
    }

    private async void ShowPreviousCard(bool animated = false)
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

                // Push the old top card behind as the back card
                oldTopCard.BatchBegin();
                oldTopCard.ZIndex = 0;
                oldTopCard.Scale = 1.0;
                oldTopCard.Opacity = 0;
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
            newTopCard.Rotation = 0;
            newTopCard.TranslationX = 0;
            newTopCard.TranslationY = -newTopCard.Y;
            newTopCard.ZIndex = 1;
            newTopCard.Opacity = 1;
            newTopCard.IsVisible = true;
            newTopCard.BatchCommit();

            // Push the old top card behind as the back card
            oldTopCard.BatchBegin();
            oldTopCard.ZIndex = 0;
            oldTopCard.Scale = 1.0;
            oldTopCard.Opacity = 0;
            oldTopCard.BatchCommit();
        }

        // Update indices
        _currentDisplayIndex--;
        _topCardIndex = newTopCardIndex;
        _itemIndex = _currentDisplayIndex + 1; // _itemIndex points to the next unloaded item
        TopItem = previousItem;
        PreviousItem = _currentDisplayIndex >= 1 ? ItemsSource[_currentDisplayIndex - 1] : null;

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

    private void SendSwiped(View sender, SwipeCardDirection direction)
    {
        var cmd = SwipedCommand;
        if (cmd != null && cmd.CanExecute(SwipedCommandParameter))
        {
            cmd.Execute(new SwipedCardEventArgs(sender.BindingContext, SwipedCommandParameter, direction));
        }

        Swiped?.Invoke(sender, new SwipedCardEventArgs(sender.BindingContext, SwipedCommandParameter, direction));
    }

    private void SendDragging(View sender, SwipeCardDirection direction, DraggingCardPosition position, double distanceDraggedX, double distanceDraggedY)
    {
        var cmd = DraggingCommand;
        if (cmd != null && cmd.CanExecute(DraggingCommandParameter))
        {
            cmd.Execute(new DraggingCardEventArgs(sender.BindingContext, DraggingCommandParameter, direction, position, distanceDraggedX, distanceDraggedY));
        }

        Dragging?.Invoke(sender, new DraggingCardEventArgs(sender.BindingContext, DraggingCommandParameter, direction, position, distanceDraggedX, distanceDraggedY));
    }

    #endregion Private Methods
}