using System.Collections.ObjectModel;
using System.Windows.Input;
using Plugin.Maui.SwipeCardView.Core;

namespace SwipeCardView.Sample.ViewModels;

public class CustomizablePageViewModel : BasePageViewModel
{
    private ObservableCollection<string> _cardItems;

    private string _topItem;

    private bool _isDraggingRightSupported;
    private bool _isDraggingLeftSupported;
    private bool _isDraggingUpSupported;
    private bool _isDraggingDownSupported;

    private bool _isSwipeRightSupported;
    private bool _isSwipeLeftSupported;
    private bool _isSwipeUpSupported;
    private bool _isSwipeDownSupported;

    private uint _threshold;

    private uint _animationLength;

    private float _backCardScale;

    private float _cardRotation;

    private bool _isLoopCards;

    private int _stackDepth;
    private double _stackOffset;
    private double _stackScaleStep;
    private StackDirection _stackDirection;
    private Color? _stackCardColor;

    public CustomizablePageViewModel()
    {
        _cardItems = new ObservableCollection<string>();
        for (var i = 1; i <= 5; i++)
        {
            _cardItems.Add($"Card {i}");
        }

        _isDraggingLeftSupported = true;
        _isDraggingRightSupported = true;
        _isDraggingUpSupported = true;
        _isDraggingDownSupported = true;

        _isSwipeLeftSupported = true;
        _isSwipeRightSupported = true;
        _isSwipeUpSupported = false;
        _isSwipeDownSupported = false;

        _isLoopCards = true;

        _threshold = 100;
        _animationLength = 250;
        _backCardScale = 0.8f;
        _cardRotation = 20;

        _stackDepth = 0;
        _stackOffset = 8;
        _stackScaleStep = 0.02;
        _stackDirection = StackDirection.Bottom;
        _stackCardColor = null;

        SwipedCommand = new Command<SwipedCardEventArgs>(OnSwipedCommand);
        DraggingCommand = new Command<DraggingCardEventArgs>(OnDraggingCommand);

        ClearItemsCommand = new Command(OnClearItemsCommand);
        AddItemsCommand = new Command(OnAddItemsCommand);
    }

    public ObservableCollection<string> CardItems
    {
        get => _cardItems;
        set
        {
            _cardItems = value;
            RaisePropertyChanged();
        }
    }

    public string TopItem
    {
        get => _topItem;
        set
        {
            _topItem = value;
            RaisePropertyChanged();
        }
    }

    public bool IsDraggingRightSupported
    {
        get => _isDraggingRightSupported;
        set
        {
            _isDraggingRightSupported = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(SupportedDraggingDirections));
        }
    }

    public bool IsDraggingLeftSupported
    {
        get => _isDraggingLeftSupported;
        set
        {
            _isDraggingLeftSupported = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(SupportedDraggingDirections));
        }
    }

    public bool IsDraggingUpSupported
    {
        get => _isDraggingUpSupported;
        set
        {
            _isDraggingUpSupported = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(SupportedDraggingDirections));
        }
    }

    public bool IsDraggingDownSupported
    {
        get => _isDraggingDownSupported;
        set
        {
            _isDraggingDownSupported = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(SupportedDraggingDirections));
        }
    }

    public SwipeCardDirection SupportedDraggingDirections => (IsDraggingRightSupported ? SwipeCardDirection.Right : SwipeCardDirection.None)
                                                            | (IsDraggingLeftSupported ? SwipeCardDirection.Left : SwipeCardDirection.None)
                                                            | (IsDraggingUpSupported ? SwipeCardDirection.Up : SwipeCardDirection.None)
                                                            | (IsDraggingDownSupported ? SwipeCardDirection.Down : SwipeCardDirection.None);

    public bool IsSwipeRightSupported
    {
        get => _isSwipeRightSupported;
        set
        {
            _isSwipeRightSupported = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(SupportedSwipeDirections));
        }
    }

    public bool IsSwipeLeftSupported
    {
        get => _isSwipeLeftSupported;
        set
        {
            _isSwipeLeftSupported = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(SupportedSwipeDirections));
        }
    }

    public bool IsSwipeUpSupported
    {
        get => _isSwipeUpSupported;
        set
        {
            _isSwipeUpSupported = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(SupportedSwipeDirections));
        }
    }

    public bool IsSwipeDownSupported
    {
        get => _isSwipeDownSupported;
        set
        {
            _isSwipeDownSupported = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(SupportedSwipeDirections));
        }
    }

    public SwipeCardDirection SupportedSwipeDirections => (IsSwipeRightSupported ? SwipeCardDirection.Right : SwipeCardDirection.None)
                                                            | (IsSwipeLeftSupported ? SwipeCardDirection.Left : SwipeCardDirection.None)
                                                            | (IsSwipeUpSupported ? SwipeCardDirection.Up : SwipeCardDirection.None)
                                                            | (IsSwipeDownSupported ? SwipeCardDirection.Down : SwipeCardDirection.None);

    public uint Threshold
    {
        get => _threshold;
        set
        {
            _threshold = value;
            RaisePropertyChanged();
        }
    }

    public uint AnimationLength
    {
        get => _animationLength;
        set
        {
            _animationLength = value;
            RaisePropertyChanged();
        }
    }

    public float BackCardScale
    {
        get => _backCardScale;
        set
        {
            _backCardScale = value;
            RaisePropertyChanged();
        }
    }

    public float CardRotation
    {
        get => _cardRotation;
        set
        {
            _cardRotation = value;
            RaisePropertyChanged();
        }
    }

    public bool IsLoopCards
    {
        get => _isLoopCards;
        set
        {
            _isLoopCards = value;
            RaisePropertyChanged();
        }
    }

    public int StackDepth
    {
        get => _stackDepth;
        set
        {
            _stackDepth = value;
            RaisePropertyChanged();
        }
    }

    public double StackOffset
    {
        get => _stackOffset;
        set
        {
            _stackOffset = value;
            RaisePropertyChanged();
        }
    }

    public double StackScaleStep
    {
        get => _stackScaleStep;
        set
        {
            _stackScaleStep = value;
            RaisePropertyChanged();
        }
    }

    public StackDirection StackDirection
    {
        get => _stackDirection;
        set
        {
            _stackDirection = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(IsStackDirectionTop));
        }
    }

    public bool IsStackDirectionTop
    {
        get => _stackDirection == StackDirection.Top;
        set
        {
            StackDirection = value ? StackDirection.Top : StackDirection.Bottom;
        }
    }

    public Color? StackCardColor
    {
        get => _stackCardColor;
        set
        {
            _stackCardColor = value;
            RaisePropertyChanged();
        }
    }

    public string StackCardColorText
    {
        get => _stackCardColor != null ? _stackCardColor.ToArgbHex() : "";
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                StackCardColor = null;
            }
            else
            {
                var hex = value.StartsWith("#") ? value : "#" + value;
                try { StackCardColor = Color.FromArgb(hex); }
                catch { /* ignore invalid color strings */ }
            }
            RaisePropertyChanged();
        }
    }

    public ICommand SwipedCommand { get; }

    public ICommand DraggingCommand { get; }

    public ICommand ClearItemsCommand { get; }

    public ICommand AddItemsCommand { get; }

    private void OnSwipedCommand(SwipedCardEventArgs eventArgs)
    {
    }

    private void OnDraggingCommand(DraggingCardEventArgs eventArgs)
    {
        switch (eventArgs.Position)
        {
            case DraggingCardPosition.Start:
                return;

            case DraggingCardPosition.UnderThreshold:
                break;

            case DraggingCardPosition.OverThreshold:
                break;

            case DraggingCardPosition.FinishedUnderThreshold:
                return;

            case DraggingCardPosition.FinishedOverThreshold:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnClearItemsCommand()
    {
        CardItems.Clear();
    }

    private void OnAddItemsCommand()
    {
        for (var i = 1; i <= 5; i++)
        {
            CardItems.Add($"Card {i}");
        }
    }
}