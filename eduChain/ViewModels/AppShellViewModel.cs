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
     private string _fullName;
    
     private ImageSource _profileImage;
    public ImageSource ProfileImage { 
                                    get {
                                        return _profileImage; 
                                        } 
                                    set {
                                         _profileImage = value; 
                                         OnPropertyChanged(nameof(ProfileImage));
                                        }
                                    }
public async Task LoadProfilePicture()
        {
            try
            {
                // Your existing code to fetch the profile picture
                byte[] profilePic = Profile.ProfilePic;

                if (profilePic != null)
                {
                    // Convert the byte array to an SKBitmap
                    using (var stream = new MemoryStream(profilePic))
                    {
                        SKBitmap bitmap = SKBitmap.Decode(stream);

                        // Set the Image Source to the decoded bitmap
                        SKImage image = SKImage.FromBitmap(bitmap);
                        ProfileImage = ImageSource.FromStream(() => image.Encode().AsStream());
                        OnPropertyChanged(nameof(ProfileImage));
                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current?.MainPage?.DisplayAlert("Error", ex.Message, "OK");
                // Handle errors
            }
        }
    
    async void ExecuteLogout()
    {
        // Handle the logout logic here
       Preferences.Default.Clear();
        MyProfileModel.Instance = null;  
        await Shell.Current.GoToAsync("//loginPage");
    }
}
