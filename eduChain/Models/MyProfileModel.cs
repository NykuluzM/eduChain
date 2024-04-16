using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using SkiaSharp;
namespace eduChain.Models;

public class MyProfileModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

   private static MyProfileModel instance;
    private static readonly object lockObject = new object();

    
    public MyProfileModel(){
        LoadProfileImage();
    }
    
        public static MyProfileModel Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new MyProfileModel();
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

    public string Email { get; set; }
    private string _firstName;
    public string FirstName { 
                                get { return _firstName; }
                                set{
                                    if(_firstName != value){
                                        _firstName = value;
                                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FirstName)));
                                        UpdateFullName();
                                    }
                                }
                            }   
    private string _lastName;
    public string LastName  { 
                                get { return _lastName; }
                                set{
                                    if(_lastName != value){
                                        _lastName = value;
                                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastName)));
                                        UpdateFullName();
                                    }
                                }
                            }
                              private string _fullName;
        public string FullName
        {
            get { return _fullName; }
            set
            {
                if (_fullName != value)
                {
                    _fullName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullName)));
                }
            }
        }

        private void UpdateFullName()
        {
            FullName = $"{FirstName} {LastName}";
        }

    private string _gender;
    public string Gender   {
                                get { return _gender; }
                                set
                                {
                                    if(_gender != value){
                                        _gender = value;
                                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Gender)));
                                    }
                                }
                            }
    private string birth_date;
    public string BirthDate {
                                get { return birth_date; }
                                set
                                {
                                    if(birth_date != value){
                                        birth_date = value;
                                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BirthDate)));
                                          // Calculate age based on birth date
                                        if (DateTime.TryParse(value, out DateTime birthDateTime))
                                        {
                                            TimeSpan ageSpan = DateTime.Now - birthDateTime;
                                            int age = (int)(ageSpan.Days / 365.25); // Approximate age in years
                                            Age = age.ToString(); // Update the Age property with the calculated age
                                        }
                                        else
                                        {
                                            // Handle invalid birth date format
                                            // For example, set Age to null or an empty string
                                            Age = null; // or Age = "";
                                        }
                                    }
                                }
                            }
private string age;
public string Age {
    get { return age; }
    set
    {
        if (age != value)
        {
            age = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Age)));
        }
    }
}
    private string _createdAt;
    public string CreatedAt {
                                get { return _createdAt; }
                                set
                                {
                                    if(_createdAt != value){
                                        _createdAt = value;
                                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CreatedAt)));
                                    }
                                }
                            }
    private string _role;
    public string Role {
                                get { return _role; }
                                set
                                {
                                    if(_role != value){
                                        _role = value;
                                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Role)));
                                    }
                                }
                            }
    public string FirebaseId { get; set; }
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
     private byte[] _profilePic;
    [AllowNull]
    public byte[] ProfilePic {
                                get { return _profilePic; }
                                set
                                {
                                    if(_profilePic != value){
                                        _profilePic = value;
                                        LoadProfileImage(); // Call the method to load the profile image when the property is set

                                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProfilePic)));
                                    }
                                }
                            }

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
                ProfileImage = "dotnet_bot.png";
            }
        }
        catch (Exception ex)
        {
            // Handle errors
            Application.Current?.MainPage?.DisplayAlert("Error", ex.Message, "OK");
        }
    }


}
