using SkiaSharp;
using LukeMauiFilePicker;
using Supabase;
using Npgsql;
using eduChain.Models;
using System.ComponentModel;
using eduChain.ViewModels;
using Plugin.Maui.Audio;
using eduChain.Services;

namespace eduChain.Views.ContentPages{
	public partial class StudentProfilePage : ContentPage
	{
        readonly IFilePickerService picker;
                private StudentProfileViewModel _viewModel;
        private StudentProfileModel _studentProfile;

        
		public StudentProfilePage()
		{
			InitializeComponent();
            picker = IPlatformApplication.Current.Services.GetRequiredService<IFilePickerService>();
            _viewModel = new StudentProfileViewModel();
            EmailLabel.Text = "Email: " + Preferences.Get("email", String.Empty);
            BindingContext = _viewModel;
            _studentProfile = StudentProfileModel.Instance;
		}
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            //var plp = IPlatformApplication.Current.Services.GetRequiredService<IAudioManager>();
            //await Shell.Current.Navigation.PushAsync(new LoadingOnePage(plp)); // Push LoadingPage
            await _viewModel.LoadProfileAsync(Preferences.Default.Get("firebase_uid", String.Empty), _studentProfile);
            _viewModel.imageBytes = null;

            //await Shell.Current.Navigation.PopAsync(); // Pop LoadingPage
        }
private void EditProfile(object sender, EventArgs e)
{

    ProfileImageBlurred.IsVisible = true;
    ProfileImage.IsVisible = false;
    editProfile.IsVisible = true;
    fName.IsReadOnly = false;
    saveButton.IsVisible = true;    
}

private async void SaveChanges(object sender, EventArgs e)
{
    ProfileImageBlurred.IsVisible = false;
    ProfileImage.IsVisible = true;
    editProfile.IsVisible = false;
    await _viewModel.UpdateProfileAsync();
}
	}
}