using SkiaSharp;
using LukeMauiFilePicker;
using Supabase;
using Npgsql;
using eduChain.Models;
using System.ComponentModel;
using eduChain.ViewModels;
using Plugin.Maui.Audio;
using eduChain.Services;
using eduChain.Models.MyProfileModels;

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
            _viewModel.PreviewImage = ImageSource.FromStream(() => new MemoryStream(UsersProfileModel.Instance.ProfilePic));

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
    EditButton.IsVisible = false;
    CancelButton.IsVisible = true;
    ProfileImageBlurred.IsVisible = true;
    ProfileImage.IsVisible = false;
    editProfile.IsVisible = true;

    saveButton.IsVisible = true;
    CancelButton.Focus();
    return;
}
private void ShowPersonal(object sender, EventArgs e)
        {
    fName.IsVisible = true;
    lName.IsVisible = true;
    age.IsVisible = true;
    gender.IsVisible = true;
    ShowButton1.IsVisible = false;
    HideButton1.IsVisible = true;
    return;
}

private void HidePersonal(object sender, EventArgs e)
        {
            fName.IsVisible = false;
            lName.IsVisible = false;
            age.IsVisible = false;
            gender.IsVisible = false;
            ShowButton1.IsVisible = true;
            HideButton1.IsVisible = false;
        }
private void CancelEditProfile(object sender, EventArgs e)
{
    EditButton.IsVisible = true;
    CancelButton.IsVisible = false;
    ProfileImageBlurred.IsVisible = false;
    ProfileImage.IsVisible = true;
    editProfile.IsVisible = false;

    saveButton.IsVisible = false;
    _viewModel.PreviewImage = ImageSource.FromStream(() => new MemoryStream(UsersProfileModel.Instance.ProfilePic));

     EditButton.Focus();
     return;
}

private void Back(object sender, EventArgs e)
        {
            Shell.Current.Navigation.PopAsync();
        }

private async void SaveChanges(object sender, EventArgs e)
{
    CancelEditProfile(sender, e);
    await _viewModel.UpdateProfileAsync();
}

        private void Button_Clicked(object sender, EventArgs e)
        {

        }
    }
}