using SwipeCardView.Sample.ViewModels;

namespace SwipeCardView.Sample.Views;

public partial class CustomizablePage : ContentPage
{
    public CustomizablePage()
    {
        InitializeComponent();
        BindingContext = new CustomizablePageViewModel();
    }
}