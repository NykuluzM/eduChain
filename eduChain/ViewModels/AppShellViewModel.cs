using  System.Windows.Input;    
namespace eduChain;

public class AppShellViewModel : ViewModelBase
{
    public ICommand LogoutCommand { get; }

    public AppShellViewModel()
    {
        LogoutCommand = new Command(ExecuteLogout);
    }
    async void ExecuteLogout()
    {
        // Handle the logout logic here
        Preferences.Default.Set("email", string.Empty);
        Preferences.Default.Set("password", string.Empty);
        Preferences.Default.Set("IsLoggedIn", false);
        await Shell.Current.GoToAsync("//loginPage");
    }
}
