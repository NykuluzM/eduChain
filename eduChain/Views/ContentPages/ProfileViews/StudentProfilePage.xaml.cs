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
using CommunityToolkit.Maui.PlatformConfiguration.AndroidSpecific;

namespace eduChain.Views.ContentPages.ProfileViews
{
    public partial class StudentProfilePage : ContentPage, IProfilePage
    {
        readonly IFilePickerService picker;
        private StudentProfileViewModel _viewModel;
        private StudentProfileModel _studentProfile;

        private string ofname;
        private string olname;
        private string ogender;
        private string obirthdate;
        private byte[] oprofile;
        public StudentProfilePage()
        {
            InitializeComponent();
            picker = IPlatformApplication.Current.Services.GetRequiredService<IFilePickerService>();
            _viewModel = new StudentProfileViewModel();
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


        private async void ShowPersonal(object sender, EventArgs e)
        {

            ShowButton1.Opacity = 1;
            ShowButton1.IsVisible = false;
            await ShowButton1.FadeTo(0, 400);

            HideButton1.Opacity = 0;
            HideButton1.IsVisible = true;
            await HideButton1.FadeTo(1, 700);


            EditButton.Opacity = 0;
            EditButton.IsVisible = true;
            await EditButton.FadeTo(1, 400);

            PersonalDetails.Opacity = 0;
            PersonalDetails.IsVisible = true;
            await PersonalDetails.FadeTo(1, 1000);

            EditButton.Focus();
        }

        private async void HidePersonal(object sender, EventArgs e)
        {
            EditButton.IsEnabled = false;
            HideButton1.IsEnabled = false;
            await PersonalDetails.FadeTo(0, 700);
            PersonalDetails.IsVisible = false;
            await EditButton.FadeTo(0, 400);
            EditButton.IsVisible = false;
            await HideButton1.FadeTo(0, 400);
            HideButton1.IsVisible = false;
            await ShowButton1.FadeTo(1, 100);
            ShowButton1.IsVisible = true;
            HideButton1.IsEnabled = true;
            EditButton.IsEnabled = true;
            EditButton.Focus();
        }
        public void CancelEditProfile(object sender, EventArgs e)
        {
            EditButton.IsVisible = true;
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
                CancelEditProfile(sender, e);
                return;
            }

            await _viewModel.UpdateProfileAsync();
            CancelEditProfile(sender, e);
            return;
        }
    }
}