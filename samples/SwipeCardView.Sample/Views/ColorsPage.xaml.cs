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
        // Use the card view (the Border with rounded corners) for color feedback,
        // not the SwipeCardView itself which is a rectangular container.
        var card = e.CardView ?? (View)sender;

        // Update page-level labels (not inside card template) to avoid
        // layout recalculation inside the Frame during Scale transforms,
        // which causes Android rendering artifacts.
        DirectionLabel.Text = e.Direction.ToString();
        PositionLabel.Text = e.Position.ToString();

        switch (e.Position)
        {
            case DraggingCardPosition.Start:
                break;

            case DraggingCardPosition.UnderThreshold:
                card.BackgroundColor = Colors.DarkTurquoise;
                break;

            case DraggingCardPosition.OverThreshold:
                switch (e.Direction)
                {
                    case SwipeCardDirection.Left:
                        card.BackgroundColor = Color.FromArgb("#FF6A4F");
                        break;

                    case SwipeCardDirection.Right:
                        card.BackgroundColor = Color.FromArgb("#63DD99");
                        break;

                    case SwipeCardDirection.Up:
                        card.BackgroundColor = Color.FromArgb("#2196F3");
                        break;

                    case SwipeCardDirection.Down:
                        card.BackgroundColor = Colors.MediumPurple;
                        break;
                }
                break;

            case DraggingCardPosition.FinishedUnderThreshold:
                card.BackgroundColor = Colors.Beige;
                break;

            case DraggingCardPosition.FinishedOverThreshold:
                card.BackgroundColor = Colors.Beige;
                DirectionLabel.Text = string.Empty;
                PositionLabel.Text = string.Empty;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnGoBackClicked(object? sender, EventArgs e)
    {
        SwipeCardView.GoBack(animated: true);
    }
}