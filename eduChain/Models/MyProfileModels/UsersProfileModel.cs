using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using SkiaSharp;
namespace eduChain.Models.MyProfileModels;

public class UsersProfileModel : INotifyPropertyChanged
{
    private static UsersProfileModel instance;
    private static readonly object lockObject = new object();
    public event PropertyChangedEventHandler? PropertyChanged;

    public static UsersProfileModel Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new UsersProfileModel();
                        instance.ProfilePic = null;
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
    private string _firebaseId;
    public string FirebaseId
    {
        get { return _firebaseId; }
        set
        {
            if (_firebaseId != value)
            {
                _firebaseId = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FirebaseId)));
            }
        }
    }
    private string _role;
    private byte[] _profilePic;
    public string Role
    {
        get { return _role; }
        set
        {
            if (_role != value)
            {
                _role = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Role)));
            }
        }
    } 
    private string _displayName;
    public string DisplayName
    {
        get { return _displayName; }
        set
        {
            if (_displayName != value)
            {
                _displayName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
            }
        }
    }
    [AllowNull]
    public byte[] ProfilePic {
                                get { return _profilePic; }
                                set
                                {
                                    if(value == null)
                                    {
                                        _profilePic = null;
                                        ProfileImage = ImageSource.FromFile("profiledefault.png");
                                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProfilePic)));
                                    }
                                    else
                                    if(_profilePic != value){
                                        _profilePic = value;
                                        LoadProfileImage(); // Call the method to load the profile image when the property is set

                                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProfilePic)));
                                    }
                                }
                            }
    private ImageSource _profileImage;
    public ImageSource ProfileImage
    { 
        get {  return _profileImage; }
        set
        {
            if (_profileImage != value)
            {
                _profileImage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProfileImage)));
            }
        }
    }           
    public DateTime CreatedAt { get; set; }

    private async void LoadProfileImage()
    {
        try
        {
            if (ProfilePic != null)
            {
                // Convert the byte array to an SKBitmap
                using (var stream = new MemoryStream(ProfilePic))
                {
                    SKBitmap bitmap = SKBitmap.Decode(stream);

                    // Set the Image Source to the decoded bitmap
                    SKImage image = SKImage.FromBitmap(bitmap);
                    ProfileImage = ImageSource.FromStream(() => image.Encode().AsStream());
                }
            } else {
                ProfileImage = ImageSource.FromFile("profiledefault.png");
            }
        }
        catch (Exception ex)
        {
            // Handle errors
            Application.Current?.MainPage?.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
