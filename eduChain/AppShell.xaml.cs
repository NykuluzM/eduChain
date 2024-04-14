namespace eduChain;
using eduChain.Views.ContentPages;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.Maui.Storage;
using Nethereum.Model;

public partial class AppShell : Shell
{
	AppShellViewModel _viewModel;

	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(Routes.RegisterPage, typeof(RegisterPage));

		Routing.RegisterRoute("forgotPasswordPage", typeof(ForgotPasswordPage));
		_viewModel = new AppShellViewModel();
		this.BindingContext = _viewModel;
		//ManipulateFlyoutItemForPlatform(SettingsItem);
		//ManipulateFlyoutItemForPlatform(IpfsItem);
		//ManipulateFlyoutItemForPlatform(HomeItem);
		//ManipulateFlyoutItemForPlatform(ProfileItem);
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
		await _viewModel.LoadProfileAsync(Preferences.Default.Get("firebase_uid", String.Empty));
		await Shell.Current.DisplayAlert("Welcome", "Welcome to eduChain", "OK");
    }

    private void ManipulateFlyoutItemForPlatform(FlyoutItem flyoutItem)
        {
            if (flyoutItem != null)
            {
                var platform = DeviceInfo.Current.Platform;
				if (DeviceInfo.Platform == DevicePlatform.iOS ||
                DeviceInfo.Platform == DevicePlatform.Android){
 					flyoutItem.SetValue(Shell.FlyoutBehaviorProperty, FlyoutBehavior.Flyout);
					return;
				}
                else {
                        flyoutItem.SetValue(Shell.FlyoutBehaviorProperty, FlyoutBehavior.Locked);
						return;
					}
			}       
        }
	private async void OnLogoutClicked(object sender, EventArgs e)
	{
		Preferences.Default.Clear();
		await Shell.Current.GoToAsync("//loginPage");
	}
	private async void ProfileTapped(object sender, EventArgs e)
	{
		await Shell.Current.Navigation.PushAsync(new MyProfilePage());
		//await Shell.Current.GoToAsync("//myProfilePage");
	}
	/*private void ExpandCollapse(object sender, TappedEventArgs e)
	{
		if(Expand.IsVisible)
		{
			var animation = new Animation((current) => 
			{
				FlyoutWidth = current;
			}, 75, 265, null);

			animation.Commit(this, "expand", finished: (value, cancelled) =>
			{
				Expand.IsVisible = false;
				Collapse.IsVisible = true;
				Name.IsVisible = true;
				Role.IsVisible = true;
			});
		} else{
			var animation = new Animation((current) => 
			{
				FlyoutWidth = current;
			}, 265, 75, null);

			animation.Commit(this, "collapse", finished: (value, cancelled) =>
			{
				Expand.IsVisible = true;
				Collapse.IsVisible = false;
				Name.IsVisible = false;
				Role.IsVisible = false;
			});	
		}
	}*/
	
}
