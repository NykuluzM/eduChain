using eduChain.Models;
using eduChain.Services;
using LukeMauiFilePicker;
using SkiaSharp;
using Npgsql;
namespace eduChain.ViewModelsx;

public class MyProfileViewModel : ViewModelBase 
{
        readonly IFilePickerService picker;
        public byte[] imageBytes { get; set;}
         Dictionary<DevicePlatform, IEnumerable<string>> FileType = new()
                {
                    { DevicePlatform.Android, new[] { "image/*" } },
                    { DevicePlatform.iOS, new[] { "public.image" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.jpeg", "public.png" } },
                    { DevicePlatform.WinUI, new[] { ".jpg", ".png" } }
                };

        public Command EditImageCommand { get; private set; }



    public MyProfileViewModel()
    {
        picker = IPlatformApplication.Current.Services.GetRequiredService<IFilePickerService>();
        EditImageCommand = new Command(async () => await EditImage());
    }

    public async Task UpdateProfileAsync(){
        await MyProfileService.Instance.UpdateUserProfileAsync(Profile);
        await UploadImageToSupabase(imageBytes,Preferences.Default.Get("firebase_uid", String.Empty));

    } 
    private async Task EditImage()
{
    try
    {
        // Select an image from the device
        var file = await picker.PickFileAsync("Select a file", FileType);
        if (file is null) { return; }
        // Set the image source aof SelectedImage
       // Convert image to byte array
                using (var stream = await file.OpenReadAsync())
                {
                    // using (var memoryStream = new MemoryStream())
                    // {
                    //     // SKBitmap bitmap = SKBitmap.Decode(stream);
                    //     // SKImage image = SKImage.FromBitmap(bitmap);
                    //     // SelectedImage.Source = ImageSource.FromStream(() => image.Encode().AsStream());
                    //                         // Decode image and convert to byte array
                        
                    // }

                    // Upload image to Supabase storage
                    SKBitmap originalBitmap = SKBitmap.Decode(stream);
                    SKBitmap resizedBitmap = originalBitmap.Resize(new SKImageInfo(250, 250), SKFilterQuality.High);

                    imageBytes = resizedBitmap.Encode(SKEncodedImageFormat.Png, 100).ToArray();

                        // Upload image to Supabase storage
                    if(Preferences.Default.Get("firebase_uid",String.Empty) != String.Empty){
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

private async Task UploadImageToSupabase(byte[] imageBytes, string firebase_id){
    //Application.Current.MainPage.DisplayAlert("Upload", "Uploading", "OK");
        if(imageBytes == null){
            return;
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
                        await Application.Current.MainPage.DisplayAlert("Upload", "Image uploaded successfully", "OK");
                        Profile.ProfilePic = imageBytes;
                    }
                }
            } catch(Exception ex){
                await Application.Current.MainPage.DisplayAlert("Error Try", $"An error occurred: {ex.Message}", "OK");
            }
            finally{
                await DatabaseManager.CloseConnectionAsync(); // Close the database connection
            }
}
} 
