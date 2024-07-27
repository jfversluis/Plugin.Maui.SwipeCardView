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
        var view = (View)sender;

        var directionLabel = view.FindByName<Label>("DirectionLabel");
        directionLabel.Text = e.Direction.ToString();

        var positionLabel = view.FindByName<Label>("PositionLabel");
        positionLabel.Text = e.Position.ToString();

        switch (e.Position)
        {
            case DraggingCardPosition.Start:
                break;

            case DraggingCardPosition.UnderThreshold:
                view.BackgroundColor = Colors.DarkTurquoise;
                break;

            case DraggingCardPosition.OverThreshold:
                switch (e.Direction)
                {
                    case SwipeCardDirection.Left:
                        view.BackgroundColor = Color.FromArgb("#FF6A4F");
                        break;

                    case SwipeCardDirection.Right:
                        view.BackgroundColor = Color.FromArgb("#63DD99");
                        break;

                    case SwipeCardDirection.Up:
                        view.BackgroundColor = Color.FromArgb("#2196F3");
                        break;

                    case SwipeCardDirection.Down:
                        view.BackgroundColor = Colors.MediumPurple;
                        break;
                }
                break;

            case DraggingCardPosition.FinishedUnderThreshold:
                view.BackgroundColor = Colors.Beige;
                break;

            case DraggingCardPosition.FinishedOverThreshold:
                view.BackgroundColor = Colors.Beige;
                directionLabel.Text = string.Empty;
                positionLabel.Text = string.Empty;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}