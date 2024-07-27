using SwipeCardView.Sample.ViewModels;

namespace SwipeCardView.Sample.Views;

public partial class SimplePage : ContentPage
{
	public SimplePage()
	{
		InitializeComponent();
		BindingContext = new SimplePageViewModel();
	}
}
