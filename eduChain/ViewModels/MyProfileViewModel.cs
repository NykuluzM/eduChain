using eduChain.Models;
using eduChain.Services;
using LukeMauiFilePicker;
using SkiaSharp;
using Npgsql;
namespace eduChain.ViewModelsx;

public class MyProfileViewModel : ViewModelBase 
{
    private readonly MyProfileService _myProfileService;
    private MyProfileModel _profile;

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
    public async Task LoadProfileAsync(string uid)
    {
            Profile = await _myProfileService.GetUserProfileAsync(uid);
    }
    
     public byte[] ConvertSKImageToByteArray(SKImage image)
        {
            using (var stream = new MemoryStream())
            {
                // Encode the SKImage to PNG format and write to the memory stream
                image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);

                // Return the byte array
                return stream.ToArray();
            }
        }
} 
