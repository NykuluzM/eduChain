using  System.Windows.Input; 
using eduChain.Models;  
using eduChain.Models.MyProfileModels;
using eduChain.Services;
using eduChain.Views.ContentPages;
namespace eduChain;

using eduChain.ViewModels.ProfileViewModels;
using SkiaSharp;

public class AppShellViewModel : ViewModelBase
{
    private static AppShellViewModel instance;
    private static readonly object lockObject = new object();
    public static AppShellViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new AppShellViewModel();
                        }
                    }
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }
   
    private readonly MyProfileService _myProfileService;
    public ICommand LogoutCommand { get; }
	
   
     private FlyoutBehavior _flyoutBehavior = FlyoutBehavior.Flyout;
   
    public AppShellViewModel()
    {
        Instance = this;
         var supabaseConnection = IPlatformApplication.Current.Services.GetRequiredService<ISupabaseConnection>();
        _myProfileService = new MyProfileService(supabaseConnection);
        LogoutCommand = new Command(ExecuteLogout);
    }
    async void ExecuteLogout()
    {
        // Handle the logout logic here
       Preferences.Default.Clear();
       MyProfileModel.Instance.ProfileImage = "profiledefault.png";
        UsersProfileModel.Instance.Role = null;

        App.Current.MainPage = new AppShell();


    }
}
