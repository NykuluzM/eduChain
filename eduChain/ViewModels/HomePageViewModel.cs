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
        await Shell.Current.GoToAsync("//loginPage");
    }
}
