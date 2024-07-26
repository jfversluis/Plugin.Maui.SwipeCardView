using SwipeCardView.Sample.Views;

namespace SwipeCardView.Sample;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private void OnSimplePageClicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new SimplePage());
	}

	private void OnTinderPageClicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new TinderPage());
	}

	private void OnCustomizablePageClicked(object sender, EventArgs e)
	{
		Navigation.PushAsync(new CustomizablePage());
	}
}
