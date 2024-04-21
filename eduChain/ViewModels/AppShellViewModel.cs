using  System.Windows.Input; 
using eduChain.Models;  
using eduChain.Services; 
namespace eduChain;
using SkiaSharp;

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
    async void ExecuteLogout()
    {
        // Handle the logout logic here
       Preferences.Default.Clear();
       MyProfileModel.Instance.ProfileImage = "profiledefault.png";
       await Shell.Current.GoToAsync("//loginPage");
    }
}
