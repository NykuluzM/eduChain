using LukeMauiFilePicker;
using eduChain.Models.MyProfileModels;
using eduChain.ViewModels.ProfileViewModels;
using eduChain.Views.Popups;
using Syncfusion.Maui.TabView;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using System.Xml.Linq;
using Microsoft.Maui.Controls;

namespace eduChain.Views.ContentPages;

public partial class OrganizationProfilePage : ContentPage, IProfilePage
{
    private string oname;
    private string otype;
    private string odname;
    private byte[] oprofile;
    readonly IFilePickerService picker;
    private OrganizationProfileViewModel _viewModel;
    private OrganizationProfileModel _orgProfile;
    private string currenttabaffiliationrequest = "organizationaffiliation";
    private string currenttabaffiliation = "organizationaffiliation";
    public OrganizationProfilePage()
	{
		InitializeComponent();
        picker = IPlatformApplication.Current.Services.GetRequiredService<IFilePickerService>();
        _viewModel = new OrganizationProfileViewModel();
        BindingContext = _viewModel;
        _orgProfile = OrganizationProfileModel.Instance;
        _viewModel.PreviewImage = ImageSource.FromStream(() => new MemoryStream(UsersProfileModel.Instance.ProfilePic));
    }
    protected override async void OnAppearing()
    {

        base.OnAppearing();
        //var plp = IPlatformApplication.Current.Services.GetRequiredService<IAudioManager>();
        //await Shell.Current.Navigation.PushAsync(new LoadingOnePage(plp)); // Push LoadingPage
        await _viewModel.LoadProfileAsync(Preferences.Default.Get("firebase_uid", String.Empty), _orgProfile);
        oname = _orgProfile.Name;
        otype = _orgProfile.Type;
        odname = _viewModel.UsersProfile.DisplayName;
        oprofile = _viewModel.UsersProfile.ProfilePic;

        //await Shell.Current.Navigation.PopAsync(); // Pop LoadingPage
    }
    public async void EditProfile(object sender, EventArgs e)
    {
        EditButton.IsVisible = false;
        dName.IsReadOnly = false;
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
    public void CancelEditProfile(object sender, EventArgs e)
    {
        EditButton.IsVisible = true;
        HideButton1.IsEnabled = true;
        dName.IsReadOnly = true;

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


        if (_orgProfile.Name == oname 
          && _orgProfile.Type == otype && _viewModel.UsersProfile.DisplayName == odname)
        {
            await DisplayAlert("Error", "No changes made", "OK");
            CancelEditProfile(sender, e);
            return;
        }

        await _viewModel.UpdateProfileAsync();
        CancelEditProfile(sender, e);
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
   
    private async void ShowForm(object sender, EventArgs e)
    {
        this.ShowPopup(new ChangePasswordPopup());
    }

}