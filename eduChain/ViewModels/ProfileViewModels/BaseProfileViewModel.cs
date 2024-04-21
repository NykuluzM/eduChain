﻿using eduChain.Models;
using eduChain.Services;
using eduChain.ViewModels;
using LukeMauiFilePicker;
using Npgsql;
using SkiaSharp;
namespace eduChain.ViewModels.ProfileViewModels;

public class BaseProfileViewModel : ViewModelBase
{
      protected readonly IFilePickerService picker;
       protected MyProfileService _myProfileService = MyProfileService.Instance;
        public byte[] imageBytes { get; set;}
         Dictionary<DevicePlatform, IEnumerable<string>> FileType = new()
                {
                    { DevicePlatform.Android, new[] { "image/*" } },
                    { DevicePlatform.iOS, new[] { "public.image" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.jpeg", "public.png" } },
                    { DevicePlatform.WinUI, new[] { ".jpg", ".png" } }
                };

        public Command EditImageCommand { get; private set; }



    public BaseProfileViewModel()
    {
        picker = IPlatformApplication.Current.Services.GetRequiredService<IFilePickerService>();
        EditImageCommand = new Command(async () => await EditImage());
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

    protected async Task UploadImageToSupabase(byte[] imageBytes, string firebase_id){
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
                        
                    UsersProfile.ProfilePic = imageBytes;
                    await Application.Current.MainPage.DisplayAlert("Success", "Profile Updated", "OK");

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