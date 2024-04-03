using CommunityToolkit.Maui.Core.Views;
using eduChain.Views;
using eduChain.Views.ContentPages;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using eduChain.Models;
using Firebase.Auth;
namespace eduChain{
public partial class App : Application
{
    private FirebaseService firebaseService;
	public App()
	{
		InitializeComponent();
		MainPage = new AppShell();
        firebaseService = FirebaseService.GetInstance();
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
            string email = Preferences.Default.Get("email", "");
            string password = Preferences.Default.Get("password", "");
            try
            {
                var firebaseAuthClient = firebaseService.GetFirebaseAuthClient();

                var userCredential = await firebaseAuthClient.SignInWithEmailAndPasswordAsync(email, password);
                await Shell.Current.GoToAsync("//homePage");
            }
            catch (FirebaseAuthException ex)
            {
                Application.Current?.MainPage?.DisplayAlert("Credentials Error", "Remembered credentials is not anymore valid", "OK");
                Preferences.Default.Clear();
                return;
            }
        }
        else
        {
            return;
        }
 
    }
}
}