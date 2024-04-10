using SkiaSharp;
using LukeMauiFilePicker;
using Supabase;
using Npgsql;
using eduChain.Models;
using System.ComponentModel;

namespace eduChain.Views.ContentPages{
	public partial class MyProfilePage : ContentPage
	{
        readonly IFilePickerService picker;
                    private readonly ISupabaseConnection _supabaseConnection;

         Dictionary<DevicePlatform, IEnumerable<string>> FileType = new()
                {
                    { DevicePlatform.Android, new[] { "image/*" } },
                    { DevicePlatform.iOS, new[] { "public.image" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.jpeg", "public.png" } },
                    { DevicePlatform.WinUI, new[] { ".jpg", ".png" } }
                };

		public MyProfilePage()
		{
			InitializeComponent();
            picker = IPlatformApplication.Current.Services.GetRequiredService<IFilePickerService>();

		}
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadProfilePicture();
        }
 private async void LoadProfilePicture()
        {
            try
            {
                // Your existing code to fetch the profile picture
                byte[] profilePic = await GetProfilePicAsync();

                if (profilePic != null)
                {
                    // Convert the byte array to an SKBitmap
                    using (var stream = new MemoryStream(profilePic))
                    {
                        SKBitmap bitmap = SKBitmap.Decode(stream);

                        // Set the Image Source to the decoded bitmap
                        SKImage image = SKImage.FromBitmap(bitmap);
                ProfileImage.Source = ImageSource.FromStream(() => image.Encode().AsStream());
                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current?.MainPage?.DisplayAlert("Error", ex.Message, "OK");
                // Handle errors
            }
        }
private async void SelectImageButton_Clicked(object sender, EventArgs e)
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

                    byte[] imageBytes = resizedBitmap.Encode(SKEncodedImageFormat.Png, 100).ToArray();
                    await Application.Current.MainPage.DisplayAlert("Upload", Preferences.Default.Get("firebase_uid", String.Empty), "OK");

                        // Upload image to Supabase storage
                    if(Preferences.Default.Get("firebase_uid",String.Empty) != String.Empty){
                        await UploadImageToSupabase(imageBytes,Preferences.Default.Get("firebase_uid", String.Empty));                
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
private async Task<byte[]> GetProfilePicAsync()
{
    string firebase_id = Preferences.Default.Get("firebase_uid", String.Empty);
    byte[] profilePic = null;

    try
    {
        await DatabaseManager.OpenConnectionAsync(); // Open the database connection

        using (var command = DatabaseManager.Connection.CreateCommand())
        {
            var parameters = command.Parameters;
            if (parameters is NpgsqlParameterCollection)
            {
                command.CommandText = "SELECT \"profile_pic\" FROM \"Users\" WHERE \"firebase_id\" = @firebase_id";
                parameters.AddWithValue("firebase_id", firebase_id);

                // Execute the query and read the result
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        // Check if the "profile_pic" column is not NULL
                        if (!reader.IsDBNull(0))
                        {
                            // Read the byte array from the result
                            profilePic = reader.GetFieldValue<byte[]>(0);
                        }
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
    }
    finally
    {
        await DatabaseManager.CloseConnectionAsync(); // Close the database connection
    }

    return profilePic;
}

private async Task UploadImageToSupabase(byte[] imageBytes, string firebase_id){
    //Application.Current.MainPage.DisplayAlert("Upload", "Uploading", "OK");
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

                    }
                }
            } catch(Exception ex){
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally{
                await DatabaseManager.CloseConnectionAsync(); // Close the database connection
                LoadProfilePicture();

            }
}
	}
}