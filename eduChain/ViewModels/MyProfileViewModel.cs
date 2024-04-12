using eduChain.Models;
using eduChain.Services;
using Npgsql;
namespace eduChain.ViewModelsx;

public class MyProfileViewModel : ViewModelBase 
{
    private readonly MyProfileService _myProfileService;
    private MyProfileModel _profile;


    public MyProfileViewModel()
    {
        var supabaseConnection = IPlatformApplication.Current.Services.GetRequiredService<ISupabaseConnection>();
        _myProfileService = new MyProfileService(supabaseConnection);
    }
    public MyProfileModel Profile
        {
            get { return _profile; }
            set
            {
                _profile = value;
                OnPropertyChanged(nameof(Profile));
            }
        }

    public async Task LoadProfileAsync(string uid)
    {
            Profile = await _myProfileService.GetUserProfileAsync(uid);
            await Shell.Current.GoToAsync("myProfilePage");
    }

} 
