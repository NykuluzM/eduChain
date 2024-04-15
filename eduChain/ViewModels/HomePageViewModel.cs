using System.ComponentModel;
using eduChain.Models;
using System.Windows.Input;
using eduChain;
using SkiaSharp;

public class HomePageViewModel : ViewModelBase
{
    public ICommand LogoutCommand { get; }

    private MyProfileModel _profile;
    public HomePageViewModel()
    {
        LogoutCommand = new Command(ExecuteLogout);

    }
    
    private async void ExecuteLogout()
    {
        // Handle the logout logic here
        Preferences.Default.Set("email", string.Empty);
        Preferences.Default.Set("password", string.Empty);
        Preferences.Default.Set("IsLoggedIn", false);
        await Shell.Current.GoToAsync("//loginPage");
    }
}
