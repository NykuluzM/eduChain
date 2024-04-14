using  System.Windows.Input; 
using eduChain.Models;  
using eduChain.Services; 
namespace eduChain;

public class AppShellViewModel : ViewModelBase
{
    private readonly MyProfileService _myProfileService;
    public ICommand LogoutCommand { get; }

    public AppShellViewModel()
    {
         var supabaseConnection = IPlatformApplication.Current.Services.GetRequiredService<ISupabaseConnection>();
        _myProfileService = new MyProfileService(supabaseConnection);
        LogoutCommand = new Command(ExecuteLogout);
    }
     private string _fullName;
    
    async void ExecuteLogout()
    {
        // Handle the logout logic here
        Preferences.Default.Set("email", string.Empty);
        Preferences.Default.Set("password", string.Empty);
        Preferences.Default.Set("IsLoggedIn", false);
        MyProfileModel.Instance = null;  
        await Shell.Current.GoToAsync("//loginPage");
    }
}
