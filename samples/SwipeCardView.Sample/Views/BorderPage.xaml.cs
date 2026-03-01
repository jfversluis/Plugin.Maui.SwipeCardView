using SwipeCardView.Sample.ViewModels;

namespace SwipeCardView.Sample.Views;

public partial class BorderPage : ContentPage
{
	public BorderPage()
	{
		InitializeComponent();
		BindingContext = new SimplePageViewModel();
	}
}
