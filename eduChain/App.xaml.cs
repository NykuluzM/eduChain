using CommunityToolkit.Maui.Core.Views;
using eduChain.Views;
using eduChain.Views.ContentPages;
namespace eduChain;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		MainPage = new AppShell();
	}
    protected override async void OnStart()
    {
        base.OnStart();

        // Check remember-me state (replace with your actual logic)
        bool isLoggedIn = Preferences.Default.Get("IsLoggedIn", false);

        // Determine the destination page based on login state

        // Navigate to the determined page
        if(isLoggedIn)
        {
            Shell.Current.GoToAsync("//homePage");
        }
        else
        {
            return;
        }
 
    }
}