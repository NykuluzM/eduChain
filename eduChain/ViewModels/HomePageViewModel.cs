using System.ComponentModel;
using System.Windows.Input;

public class HomePageViewModel
{
    public ICommand LogoutCommand { get; }

    public HomePageViewModel()
    {
        LogoutCommand = new Command(ExecuteLogout);
    }

    private async void ExecuteLogout()
    {
        // Handle the logout logic here
        Preferences.Set("email", string.Empty);
        Preferences.Set("password", string.Empty);
        Preferences.Set("IsLoggedIn", false);
        await Shell.Current.GoToAsync("//loginPage");
    }
}
