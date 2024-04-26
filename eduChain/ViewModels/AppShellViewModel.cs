using  System.Windows.Input; 
using eduChain.Models;  
using eduChain.Services; 
namespace eduChain;
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
    public FlyoutBehavior FlyoutBehaviors
    {
        get => _flyoutBehavior;
        set
        {
            _flyoutBehavior = value;
            if(value == FlyoutBehavior.Locked)
            {
                IsMenuPresented = false;
            }
            else if(value == FlyoutBehavior.Flyout){
                IsMenuPresented = true;
            }
    
            OnPropertyChanged(nameof(FlyoutBehaviors));
        }
    }

    private bool _isMenuPresented = true;
    public bool IsMenuPresented
    {
        get { return _isMenuPresented;}
        set
        {
            _isMenuPresented = value;
            OnPropertyChanged(nameof(IsMenuPresented));
        }
    }

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
       FirebaseAuthService.GetInstance().GetFirebaseAuthClient().SignOut();
       await Shell.Current.GoToAsync("//loginPage");
    }
}
