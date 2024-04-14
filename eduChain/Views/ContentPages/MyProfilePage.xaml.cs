using SkiaSharp;
using LukeMauiFilePicker;
using Supabase;
using Npgsql;
using eduChain.Models;
using System.ComponentModel;
using eduChain.ViewModels;
using eduChain.ViewModelsx;
using Plugin.Maui.Audio;
using eduChain.Services;

namespace eduChain.Views.ContentPages{
	public partial class MyProfilePage : ContentPage
	{
        readonly IFilePickerService picker;
                private MyProfileViewModel _viewModel;


        
		public MyProfilePage()
		{
			InitializeComponent();
            picker = IPlatformApplication.Current.Services.GetRequiredService<IFilePickerService>();
            _viewModel = new MyProfileViewModel();
            EmailLabel.Text = "Email: " + Preferences.Get("email", String.Empty);
            BindingContext = _viewModel;
		}
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            //var plp = IPlatformApplication.Current.Services.GetRequiredService<IAudioManager>();
            //await Shell.Current.Navigation.PushAsync(new LoadingOnePage(plp)); // Push LoadingPage
            await _viewModel.LoadProfileAsync(Preferences.Default.Get("firebase_uid", String.Empty));
            await _viewModel.LoadProfilePicture();

            //await Shell.Current.Navigation.PopAsync(); // Pop LoadingPage
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

private void BlurImageButton_Clicked(object sender, EventArgs e)
{
    ProfileImageBlurred.IsVisible = true;
    ProfileImage.IsVisible = false;
    editProfile.IsVisible = true;
}
private void UnBlurImageButton_Clicked(object sender, EventArgs e)
{
    ProfileImageBlurred.IsVisible = false;
    ProfileImage.IsVisible = true;
    editProfile.IsVisible = false;  
}

	}
}