using CommunityToolkit.Maui.Behaviors;
using WalletConnectSharp;
namespace eduChain.Views.ContentPages;
public partial class WebConnectPage : ContentPage
{
	public WebConnectPage()
	{
		InitializeComponent();
	}

	private async void LoadWebpage()
	{
		string url = Preferences.Get("SchoolWebpageURL", "");
		if (!string.IsNullOrEmpty(url))
		{
			webView.Source = new UrlWebViewSource { Url = url };
			await DisplayAlert("Info", "Loaded School Webpage", "OK");
		}
		else
		{
			await DisplayAlert("Error", "School Webpage URL not set.", "OK");
		}
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		LoadWebpage();
	}
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
}