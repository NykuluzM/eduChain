using CommunityToolkit.Maui.Core.Views;
using eduChain.Views;
using eduChain.Views.ContentPages;
namespace eduChain;
using Microsoft.Maui.Storage;
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

        // Call the navigation method asynchronously
        await NavigateToLoginPageOrHomePageAsync();
    }
    public async Task NavigateToLoginPageOrHomePageAsync()
    {
 
        bool isLoggedIn = Preferences.Default.Get("IsLoggedIn", false);

        // Determine the destination page based on the authentication state
        string destinationPage = isLoggedIn ? "//homePage" : "//loginPage";

        // Navigate to the determined destination page
        await NavigateToPageAsync(destinationPage);

    }
    private async Task NavigateToPageAsync(string pageUri)
    {
        try
        {
            Page page = null;

            // Determine the type of page based on the pageUri
            switch (pageUri)
            {
                case "//homePage":
                    page = new HomePage(); // Instantiate the appropriate page type (replace HomePage with your actual home page class)
                    break;
                case "//loginPage":
                    page = new LoginPage(); // Instantiate the appropriate page type (replace LoginPage with your actual login page class)
                    break;
                // Add additional cases for other page URIs if needed
                default:
                    // Handle unknown page URIs
                    break;
            }

            if (page != null)
            {
                // Navigate to the determined page
                await Application.Current.MainPage.Navigation.PushAsync(page);
            }
            else
            {
                // Handle the case where page is null (unknown page URI)
                // You can throw an exception, log an error, or handle it as needed
            }
        }
        catch (Exception ex)
        {
            // Output the error to the debug console or handle it as needed
            System.Diagnostics.Debug.WriteLine($"An error occurred during navigation: {ex.Message}");
        }
    }
}