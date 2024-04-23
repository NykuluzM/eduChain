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
using CommunityToolkit.Maui.Alerts;

using Org.BouncyCastle.Utilities;
using CommunityToolkit.Maui.Core;

namespace eduChain.Views.ContentPages.ProfileViews{
	public partial class StudentProfilePage : ContentPage, IProfilePage
	{
        readonly IFilePickerService picker;
                private StudentProfileViewModel _viewModel;
        private StudentProfileModel _studentProfile;

        private  string ofname;
        private  string olname;
        private  string ogender;
        private string obirthdate;
        private  byte[] oprofile;
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
            ofname = _studentProfile.FirstName;
            olname = _studentProfile.LastName;
            ogender = _studentProfile.Gender;
            obirthdate = _studentProfile.BirthDate;
            oprofile = _viewModel.UsersProfile.ProfilePic;
            //await Shell.Current.Navigation.PopAsync(); // Pop LoadingPage
        }
 public void EditPicture(object sender, EventArgs e)
        {
            editProfile.IsVisible = true;
            CancelProfile.IsVisible = true;
            SaveProfile.IsVisible = true;
            ProfileChange.IsVisible = false;
            ProfileImageBlurred.IsVisible = true;
            ProfileImage.IsVisible = false;
        }
        public async void EditProfile(object sender, EventArgs e)
        {
            EditButton.IsVisible = false;
  
                    HideButton1.IsEnabled = false;
                    var cancellationTokenSource = new CancellationTokenSource();
                    var text = "Hide Button is Disabled, Either Cancel or Save";
                    var duration = ToastDuration.Long;
                    var fontSize = 14;
                    var toast = Toast.Make(text, duration, fontSize);
                    await toast.Show(cancellationTokenSource.Token);


                    StateButtons.IsVisible = true;
            StateButtons.Focus();
            return;
            
        }
        public void CancelProfileChange(object sender, EventArgs e)
        {
            CancelProfile.IsVisible = false;
            SaveProfile.IsVisible = false;
            ProfileChange.IsVisible = true;
            ProfileImageBlurred.IsVisible = false;
            ProfileImage.IsVisible = true;
            editProfile.IsVisible = false;
            _viewModel.imageBytes = null;
            _viewModel.PreviewImage = ImageSource.FromStream(() => new MemoryStream(_viewModel.UsersProfile.ProfilePic));

            return;
        }
        public async void SaveProfileChange(object sender, EventArgs e)
        {
            if(_viewModel.imageBytes == null)
            {
                await DisplayAlert("Error", "No changes made", "OK");
                return;
            }
            await _viewModel.UpdateProfilePicture();
            CancelProfileChange(sender, e);

        }
        private void ShowPersonal(object sender, EventArgs e)
        {
            PersonalDetails.IsVisible = true;
            ShowButton1.IsVisible = false;
            HideButton1.IsVisible = true;
            EditButton.IsVisible = true;
            EditButton.Focus();
        }

        private void HidePersonal(object sender, EventArgs e)
        {
            PersonalDetails.IsVisible = false;
            EditButton.IsVisible = false;
            ShowButton1.IsVisible = true;
            HideButton1.IsVisible = false;
            EditButton.Focus(); 
        }   
        public void CancelEditProfile(object sender, EventArgs e)
        {
            EditButton.IsVisible = true;
            ProfileImageBlurred.IsVisible = false;
            ProfileImage.IsVisible = true;
            HideButton1.IsEnabled = true;

            StateButtons.IsVisible = false;

             EditButton.Focus();
             return;
        }

        public void Back(object sender, EventArgs e)
        {
            Shell.Current.Navigation.PopAsync();
        }

        public async void SaveChanges(object sender, EventArgs e)
        {
          

            if (_studentProfile.LastName == olname && _studentProfile.FirstName == ofname
              && _studentProfile.BirthDate == obirthdate && _studentProfile.Gender == ogender)
            {
                await DisplayAlert("Error", "No changes made", "OK");
                return;
            }

            await _viewModel.UpdateProfileAsync();
            CancelEditProfile(sender, e);

        }
    }
}