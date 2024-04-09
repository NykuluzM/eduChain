namespace eduChain;
using eduChain.Views.ContentPages;
using Microsoft.Maui.Storage;
using Nethereum.Model;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(Routes.RegisterPage, typeof(RegisterPage));
		Routing.RegisterRoute("forgotPasswordPage", typeof(ForgotPasswordPage));
		this.BindingContext = new AppShellViewModel();
		//ManipulateFlyoutItemForPlatform(SettingsItem);
		//ManipulateFlyoutItemForPlatform(IpfsItem);
		//ManipulateFlyoutItemForPlatform(HomeItem);
		//ManipulateFlyoutItemForPlatform(ProfileItem);
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
		Preferences.Default.Set("IsLoggedIn", false);
		await Shell.Current.GoToAsync("//loginPage");
	}
	private async void ProfileTapped(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//myProfilePage");
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
