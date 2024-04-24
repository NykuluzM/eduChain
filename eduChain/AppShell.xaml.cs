namespace eduChain;
using eduChain.Views.ContentPages;
using eduChain.Views.ContentPages.ProfileViews;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.Maui.Storage;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using eduChain.Models;
using Nethereum.Model;

public partial class AppShell : Shell
{
    bool repeat = true;
	AppShellViewModel _viewModel;

	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(Routes.RegisterPage, typeof(RegisterPage));

		Routing.RegisterRoute("forgotPasswordPage", typeof(ForgotPasswordPage));
		_viewModel = new AppShellViewModel();
		this.BindingContext = _viewModel;
	}
	private async void ProfileTapped(object sender, EventArgs e)
	{
        //fly1.On<WinUI>.SetIsEnabled(false);
        if (Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.Android || Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.iOS)
        {
            fly1.IsEnabled = false;
            fly2.IsEnabled = false;
            fly3.IsEnabled = false;
            pTap.IsEnabled = false;
        }
       
        if (!repeat )
        {
            return;
        }
        switch(Preferences.Default.Get("Role", String.Empty))
        {
            case "Student":
                repeat = false;

                if (Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.Android || Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    await Shell.Current.Navigation.PushModalAsync(new StudentProfilePage(), false)
                      .ContinueWith(async task =>
                      {

                          // Ensure navigation succeeded (Handle errors if needed) 
                          if (task.IsCompletedSuccessfully)
                          {
                              await Task.Delay(500); // Delay of 500 milliseconds (adjust as needed)
                              pTap.IsEnabled = true;
                              fly1.IsEnabled = true;
                              fly2.IsEnabled = true;
                              fly3.IsEnabled = true;

                          }
                          repeat = true;
                      });
                }
                else
                {
					try{
						await Shell.Current.Navigation.PushAsync(new StudentProfilePage(), false);
											repeat = true;
					}
					catch(Exception ex){
						await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
					}
                   

                }
                break;
                case "Organization":
                    repeat = false;
                    if (Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.Android || Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.iOS)
                {
                        await Shell.Current.Navigation.PushModalAsync(new OrganizationProfilePage(), false)
                          .ContinueWith(async task =>
                          {
                              // Ensure navigation succeeded (Handle errors if needed) 
                              if (task.IsCompletedSuccessfully)
                              {
                                  await Task.Delay(500); // Delay of 500 milliseconds (adjust as needed)
                                  pTap.IsEnabled = true;
                                  fly1.IsEnabled = true;
                                  fly2.IsEnabled = true;
                                  fly3.IsEnabled = true;

                              }
                              repeat = true;
                          });
                    }
                    else
                    {
                        await Shell.Current.Navigation.PushAsync(new OrganizationProfilePage(), false);
                        repeat = true;

                    }
                    break;
                case "Guardian":
                    repeat = false;
                    if (Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.Android || Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.iOS)
                {
                        await Shell.Current.Navigation.PushModalAsync(new GuardianProfilePage(), false)
                          .ContinueWith(async task =>
                          {
                              // Ensure navigation succeeded (Handle errors if needed) 
                              if (task.IsCompletedSuccessfully)
                              {
                                  await Task.Delay(500); // Delay of 500 milliseconds (adjust as needed)
                                  pTap.IsEnabled = true;
                                  fly1.IsEnabled = true;
                                  fly2.IsEnabled = true;
                                  fly3.IsEnabled = true;

                              }
                              repeat = true;
                          });
                    }
                    else
                {
                        await Shell.Current.Navigation.PushAsync(new GuardianProfilePage(), false);
                        repeat = true;

                    }
                    break;  
        }
		
	}
	private void ExitFlyout(object sender, EventArgs e)
	{
        _viewModel.FlyoutBehaviors = FlyoutBehavior.Flyout;
        Shell.Current.FlyoutIsPresented = false;
    }
    private void LockFlyout(object sender, EventArgs e)
    {
        _viewModel.FlyoutBehaviors = FlyoutBehavior.Locked;    
    }
    protected override void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);
		NavBarHasShadowProperty.Equals(false);
    }
	protected override void OnNavigated(ShellNavigatedEventArgs args)
	{
        base.OnNavigated(args);
        NavBarHasShadowProperty.Equals(false);
    }
}
