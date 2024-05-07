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
using CommunityToolkit.Maui.Views;
using Syncfusion.Maui.DataGrid;
using Syncfusion.Maui.PullToRefresh;
using eduChain.Views.Popups;
using Syncfusion.Maui.TabView;

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
        private string currenttabaffiliationrequest = "organizationaffiliation";
        private string currenttabaffiliation = "organizationaffiliation";
        public StudentProfilePage()
        {
            InitializeComponent();
            picker = IPlatformApplication.Current.Services.GetRequiredService<IFilePickerService>();
            _viewModel = new StudentProfileViewModel();
            BindingContext = _viewModel;
            _studentProfile = StudentProfileModel.Instance;
            _viewModel.PreviewImage = ImageSource.FromStream(() => new MemoryStream(UsersProfileModel.Instance.ProfilePic));
        }
        private async void Button_Example(object sender, EventArgs e)
        {
            await AffiliationsDatabaseService.Instance.GetAffiliatedOrganizationTo(_studentProfile.FirebaseId);

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
            _viewModel.InitializeAsync();

            //await Shell.Current.Navigation.PopAsync(); // Pop LoadingPage
        }
        private async void ShowForm(object sender, EventArgs e)
        {
            this.ShowPopup(new ChangePasswordPopup());
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
        private async void Tab1Change(object sender, TabSelectionChangedEventArgs e)
        {
            var selectedItem = e.NewIndex;
            switch (selectedItem)
            {
                case 0:
                    OrgUIDButton.IsVisible = true;
                    StudUIDButton.IsVisible = false;
                    OrgUIDRequest.IsVisible = true;
                    StudUIDRequest.IsVisible = false;
                    currenttabaffiliation = "organizationaffiliation";
                    break;
                case 1:
                    OrgUIDButton.IsVisible = false;
                    StudUIDButton.IsVisible = true;
                    OrgUIDRequest.IsVisible = false;
                    StudUIDRequest.IsVisible = true;
                    currenttabaffiliation = "studentaffiliation";
                    break;
            }
        }
        private async void Tab2Change(object sender, TabSelectionChangedEventArgs e)
        {
               var selectedItem = e.NewIndex;
            switch (selectedItem)
            {
                case 0:
                    currenttabaffiliationrequest = "organizationaffiliation";
                    break;
                case 1:
                    currenttabaffiliationrequest = "studentaffiliation";
                    break;
            }
        }


        private async void RequestAffiliation(object sender, EventArgs e)
        {
            var s = (Button)sender;
            switch (s.ClassId)
            {
                case "OrgAff":
                    var res = await _viewModel.UIDVerify(OrgUIDRequest.Text, "org");
                    if(res == null || res == false)
                    {
                        await DisplayAlert("Error", "Organization not found, Please Enter a correct UID", "OK");
                    }
                    else {
                        await _viewModel.SendAffRequest(OrgUIDRequest.Text);
                        _viewModel.InitializeAsync();

                    }
                    OrgUIDRequest.Text = "";
                    break;
                case "StudAff":
                    var ress = await _viewModel.UIDVerify(StudUIDRequest.Text, "stud");
                    if (ress == null || ress == false)
                    {
                        await DisplayAlert("Error", "Student not found, Please Enter a correct UID", "OK");
                    }
                    else
                    {
                        await _viewModel.SendAffRequest(StudUIDRequest.Text);
                        _viewModel.InitializeAsync();

                    }
                    StudUIDRequest.Text = "";
                    break;

            }

        }
        private async void AcceptedAffiliation(object sender, EventArgs e)
        {
            if(currenttabaffiliationrequest == "organizationaffiliation")
            {
                if(affireqorg.SelectedRows == null || affireqorg.SelectedRows.Count < 1)
                {
                    await DisplayAlert("Error", "Please select an affiliation request", "OK");
                    return;
                }
                else
                {
                    await _viewModel.AcceptAffiliations(affireqorg.SelectedRows);
                    _viewModel.InitializeAsync();
                }
            }
            else
            {
                if(affireqstud.SelectedRows == null || affireqstud.SelectedRows.Count < 1)
                {
                    await DisplayAlert("Error", "Please select an affiliation request", "OK");
                    return;
                }
                else
                {
                    await _viewModel.AcceptAffiliations(affireqstud.SelectedRows);
                    _viewModel.InitializeAsync();
                }
            }
        }

        private async void RejectedAffiliation(object sender, EventArgs e)
        {
            if(currenttabaffiliationrequest == "organizationaffiliation")
            {
                if(affireqorg.SelectedRows == null || affireqorg.SelectedRows.Count < 1)
                {
                    await DisplayAlert("Error", "Please select an affiliation request", "OK");
                    return;
                }
                else
                {
                    await _viewModel.RejectAffiliations(affireqorg.SelectedRows);
                    _viewModel.InitializeAsync();
                }
            }
            else
            {
                if(affireqstud.SelectedRows == null || affireqstud.SelectedRows.Count < 1)
                {
                    await DisplayAlert("Error", "Please select an affiliation request", "OK");
                    return;
                }
                else
                {
                    await _viewModel.RejectAffiliations(affireqstud.SelectedRows);
                    _viewModel.InitializeAsync();
                }
            }   
        }

        private async void RemoveAffiliations(object sender, EventArgs e)
        {
            if(currenttabaffiliation == "organizationaffiliation")
            {
                if(affiorg.SelectedRows == null || affiorg.SelectedRows.Count < 1)
                {
                    await DisplayAlert("Error", "Please select an affiliation request", "OK");
                    return;
                }
                else
                {
                    await _viewModel.RemoveAffiliations(affiorg.SelectedRows);
                    _viewModel.InitializeAsync();
                }
            }
            else
            {
                if(affistud.SelectedRows == null || affistud.SelectedRows.Count < 1)
                {
                    await DisplayAlert("Error", "Please select an affiliation request", "OK");
                    return;
                }
                else
                {
                    await _viewModel.RemoveAffiliations(affistud.SelectedRows);
                    _viewModel.InitializeAsync();
                }
            }   
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