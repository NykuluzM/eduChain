using eduChain.Models;
using eduChain.Models.MyProfileModels;
using eduChain.Services;
using eduChain.ViewModels;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using LukeMauiFilePicker;
using Npgsql;
using SkiaSharp;
namespace eduChain.ViewModels.ProfileViewModels;

public class BaseProfileViewModel : ViewModelBase
{
       protected MyProfileService _myProfileService = MyProfileService.Instance;
        public byte[] imageBytes { get; set;}
        private ImageSource _previewImage;
        public ImageSource PreviewImage
    {
            get { return _previewImage; }
        set { SetProperty(ref _previewImage, value); }
    }
         Dictionary<DevicePlatform, IEnumerable<string>> FileType = new()
                {
                    { DevicePlatform.Android, new[] { "image/*" } },
                    { DevicePlatform.iOS, new[] { "public.image" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.jpeg", "public.png" } },
                    { DevicePlatform.WinUI, new[] { ".jpg", ".png" } }
                };

        public Command EditImageCommand { get; private set; }


    private async void ReadFirebaseAdminSdk()
    {
        var stream = await FileSystem.OpenAppPackageFileAsync("admin_sdk.json");
        var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync();

        // Check if FirebaseApp has already been created
        FirebaseApp existingApp = FirebaseApp.DefaultInstance;
        if (existingApp != null)
        {
            // Update the credential of the existing FirebaseApp
            existingApp.Options.Credential = GoogleCredential.FromJson(json);
        }
        else
        {
            // Create FirebaseApp with the provided options
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(json)
            });
        }
    }
    public BaseProfileViewModel()
    {
        EditImageCommand = new Command(async () => await EditImage());
        ReadFirebaseAdminSdk();
    }

    protected async Task EditImage()
    {
        try
        {
            // Select an image from the device
            var file = await picker.PickFileAsync("Select a file", FileType);
            if (file is null) { return; }
        // Convert image to byte array
                    using (var stream = await file.OpenReadAsync())
                    {
                        SKBitmap originalBitmap = SKBitmap.Decode(stream);
                        SKBitmap resizedBitmap = originalBitmap.Resize(new SKImageInfo(250, 250), SKFilterQuality.High);

                        imageBytes = resizedBitmap.Encode(SKEncodedImageFormat.Png, 100).ToArray();
                  
                        PreviewImage = ImageSource.FromStream(() => new MemoryStream(imageBytes));

                // Upload image to Supabase storage
                if (Preferences.Default.Get("firebase_uid",String.Empty) != String.Empty){
                        } else {
                            await Application.Current.MainPage.DisplayAlert("Error", "Please login to upload image", "OK");
                            await Shell.Current.GoToAsync("//loginPage");
                        }
                } 
            }
        catch (Exception ex)
        {
            Application.Current?.MainPage?.DisplayAlert("No Connection", ex.Message, "OK");
            // Handle errors
        }
    }
   
    protected async Task<int> UploadImageToSupabase(byte[] imageBytes, string firebase_id){
        if(imageBytes == null){
            await Application.Current.MainPage.DisplayAlert("Error", "Please select an image", "OK");

            return 0;
        }
        try{
            await DatabaseManager.OpenConnectionAsync(); // Open the database connection
            using (var command = DatabaseManager.Connection.CreateCommand()){
                var parameters = command.Parameters;
                if(parameters is NpgsqlParameterCollection){
                    parameters.AddWithValue("firebase_id", firebase_id);
                    parameters.AddWithValue("profile_pic", imageBytes);
                    command.CommandText = "UPDATE \"Users\" SET \"profile_pic\" = @profile_pic WHERE \"firebase_id\" = @firebase_id";
                    await command.ExecuteNonQueryAsync();
                        
                    UsersProfile.ProfilePic = imageBytes;
                    await Shell.Current.DisplayAlert("Success", "Profile picture updated", "OK");

                }
            }
            await DatabaseManager.CloseConnectionAsync(); // Close the database connection

            return 1;
        } catch(Exception ex){
            await Application.Current.MainPage.DisplayAlert("Error Try", $"An error occurred: {ex.Message}", "OK");
            await DatabaseManager.CloseConnectionAsync(); // Close the database connection

            return 0;
        }
        
        
    }
}
