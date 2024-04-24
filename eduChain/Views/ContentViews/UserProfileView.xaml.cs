using LukeMauiFilePicker;
using eduChain.Models.MyProfileModels;

namespace eduChain.Views.ContentViews;

public partial class UserProfileView : ContentView
{
    readonly IFilePickerService picker;

    private StudentProfileViewModel _viewModel;

    public UserProfileView()
	{
		InitializeComponent();
        picker = IPlatformApplication.Current.Services.GetRequiredService<IFilePickerService>();
        _viewModel = new StudentProfileViewModel();
        EmailLabel.Text = "Email: " + Preferences.Get("email", String.Empty);
        BindingContext = _viewModel;
        if (_viewModel.UsersProfile.ProfilePic != null)
        {
            _viewModel.PreviewImage = ImageSource.FromStream(() => new MemoryStream(_viewModel.UsersProfile.ProfilePic));
        }
        else
        {
            _viewModel.PreviewImage = ImageSource.FromFile("profiledefault.png");
        }
    }

    public static readonly BindableProperty UserRoleProperty = BindableProperty.Create(
        nameof(UserRole), typeof(string), typeof(UserProfileView), default(string));

    public string UserRole
    {
        get => (string)GetValue(UserRoleProperty);
        set => SetValue(UserRoleProperty, value);
    }
    public static readonly BindableProperty FirebaseIdProperty = BindableProperty.Create(
          nameof(FirebaseId), typeof(string), typeof(UserProfileView), default(string));

    public string FirebaseId
    {
        get => (string)GetValue(FirebaseIdProperty);
        set => SetValue(FirebaseIdProperty, value);
    }

    public static readonly BindableProperty EmailProperty = BindableProperty.Create(
        nameof(Email), typeof(string), typeof(UserProfileView), default(string));

    public string Email
    {
        get => (string)GetValue(EmailProperty);
        set => SetValue(EmailProperty, value);
    }

    public static readonly BindableProperty ProfileImageSourceProperty = BindableProperty.Create(
        nameof(ProfileImageSource), typeof(ImageSource), typeof(UserProfileView), default(ImageSource));

    public ImageSource ProfileImageSource
    {
        get => (ImageSource)GetValue(ProfileImageSourceProperty);
        set => SetValue(ProfileImageSourceProperty, value);
    }
    private void Back(Object sender,EventArgs args)
    {
        Shell.Current.Navigation.PopModalAsync();
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
    public void CancelProfileChange(object sender, EventArgs e)
    {
        CancelProfile.IsVisible = false;
        SaveProfile.IsVisible = false;
        ProfileChange.IsVisible = true;
        ProfileImageBlurred.IsVisible = false;
        ProfileImage.IsVisible = true;
        editProfile.IsVisible = false;
        _viewModel.imageBytes = null;
        if(_viewModel.UsersProfile.ProfilePic != null)
        {
            _viewModel.PreviewImage = ImageSource.FromStream(() => new MemoryStream(_viewModel.UsersProfile.ProfilePic));
        } else
        {
            _viewModel.PreviewImage = ImageSource.FromFile("profiledefault.png");  
        }

        return;
    }
    public async void SaveProfileChange(object sender, EventArgs e)
    {
        if (_viewModel.imageBytes == null)
        {
            await Shell.Current.DisplayAlert("Error", "No changes made", "OK");
           // await DisplayAlert("Error", "No changes made", "OK");
            CancelProfileChange(sender, e);
            return;
        }
        await _viewModel.UpdateProfilePicture();
        CancelProfileChange(sender, e);

    }
}