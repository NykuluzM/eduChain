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
}