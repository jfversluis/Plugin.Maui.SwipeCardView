using Plugin.Maui.SwipeCardView.Core;
using SwipeCardView.Sample.ViewModels;

namespace SwipeCardView.Sample.Views;

public partial class ColorsPage : ContentPage
{
    public ColorsPage()
    {
        InitializeComponent();
        BindingContext = new ColorsPageViewModel();

        SwipeCardView.Dragging += OnDragging;
    }

    private void OnDragging(object sender, DraggingCardEventArgs e)
    {
        // Update page-level labels (not inside card template) to avoid
        // layout recalculation inside the Frame during Scale transforms,
        // which causes Android rendering artifacts.
        DirectionLabel.Text = e.Direction.ToString();
        PositionLabel.Text = e.Position.ToString();

        // Apply color to the card's Border (rounded corners) instead of
        // the SwipeCardView container (square corners).
        var border = GetCardBorder(e.CardView);

        switch (e.Position)
        {
            case DraggingCardPosition.Start:
                SetCardColor(border, Color.FromArgb("#F8F8F8"));
                break;

            case DraggingCardPosition.UnderThreshold:
                SetCardColor(border, Colors.DarkTurquoise);
                break;

            case DraggingCardPosition.OverThreshold:
                switch (e.Direction)
                {
                    case SwipeCardDirection.Left:
                        SetCardColor(border, Color.FromArgb("#FF6A4F"));
                        break;

                    case SwipeCardDirection.Right:
                        SetCardColor(border, Color.FromArgb("#63DD99"));
                        break;

                    case SwipeCardDirection.Up:
                        SetCardColor(border, Color.FromArgb("#2196F3"));
                        break;

                    case SwipeCardDirection.Down:
                        SetCardColor(border, Colors.MediumPurple);
                        break;
                }
                break;

            case DraggingCardPosition.FinishedUnderThreshold:
                SetCardColor(border, Color.FromArgb("#F8F8F8"));
                break;

            case DraggingCardPosition.FinishedOverThreshold:
                SetCardColor(border, Color.FromArgb("#F8F8F8"));
                DirectionLabel.Text = string.Empty;
                PositionLabel.Text = string.Empty;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static Border? GetCardBorder(View? cardView)
    {
        if (cardView is ContentView cv && cv.Content is Border border)
            return border;
        if (cardView is Border b)
            return b;
        return null;
    }

    private static void SetCardColor(Border? border, Color color)
    {
        if (border != null)
            border.BackgroundColor = color;
    }

    private void OnGoBackClicked(object? sender, EventArgs e)
    {
        SwipeCardView.GoBack(animated: true);
    }
}