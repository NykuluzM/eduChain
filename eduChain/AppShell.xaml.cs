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
    public void TriggerLayout(string layout){
        if(layout == "collapsed"){
           
            Collapse_Clicked(this, new EventArgs());
        }
        else{
            Shell.Current.DisplayAlert("Layout", "Expanded", "OK");
        }
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
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Locked;
        if (Expand.IsVisible)
        {
            Expand.IsVisible = true;
            Lock.IsVisible = true;
            Unlock.IsVisible = true;
            Collapse.IsVisible = false;
        }
        else
        {
            Lock.IsVisible = false;
            Minimize.IsVisible = false;
            Unlock.IsVisible = true;
            Collapse.IsVisible = true;
            
        }
    }
    protected override void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);
		NavBarHasShadowProperty.Equals(false);
        
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
    protected override void OnNavigated(ShellNavigatedEventArgs args)
	{
        base.OnNavigated(args);
        NavBarHasShadowProperty.Equals(false);

    }

   
    private void Collapse_Clicked(object sender, EventArgs e)
    {
        Unlock.IsVisible = false;
        if (Microsoft.Maui.Devices.DeviceInfo.Platform != DevicePlatform.Android)
        {
            Expand.BorderWidth = 0;
        }
        Expand.RotateTo(90, 500, new Easing(t => t));
        Minimize.IsVisible = false;
        Expand.IsVisible = true;
        Collapse.IsVisible = false;
        Logout.IsVisible = false;
        Name.IsVisible = false;
        RoleVal.IsVisible = false;
        collapse1.WidthRequest = 35;
        collapse1.HeightRequest = 35;
        Lock.Margin = new Thickness(0, 0, 0, 80);
        Unlock.Margin = new Thickness(0,15,0,0);
        LabelVal.IsVisible = true;
        if (Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.WinUI  )
        {
            collapse1.WidthRequest = 45;
            collapse1.HeightRequest = 45;
            collapse1.Margin = new Thickness(17, 10, 0, 10);
            Shell.Current.FlyoutWidth = 85;
        } else if (Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.macOS || Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.MacCatalyst){
            collapse1.WidthRequest = 45;
            collapse1.HeightRequest = 45;
            collapse1.Margin = new Thickness(17, 10, 0, 10);
            Shell.Current.FlyoutWidth = 85;
        }
        else
        {
            collapse1.HorizontalOptions = LayoutOptions.Center;
            collapse1.VerticalOptions = LayoutOptions.Center;
            collapse1.Margin = new Thickness(10, 0, 0, 0);

            LabelVal.FontSize = 8;
            LabelVal.HorizontalOptions = LayoutOptions.Center;
            LabelVal.Margin = new Thickness(10, 0, 0, 0);

            Shell.Current.FlyoutWidth = 60;

        }
        Unlock.IsVisible = true;
        Lock.IsVisible = true;
    }  
    private void Expand_Clicked(object sender, EventArgs e)
    {
       Shell.Current.FlyoutWidth = 270;
        Collapse.IsVisible = true;
        if(Shell.Current.FlyoutBehavior == FlyoutBehavior.Locked){
            Unlock.IsVisible = true;
            Lock.IsVisible = false;
        } else {
            Unlock.IsVisible = false;
            Lock.IsVisible = true;
        }
       

        Minimize.IsVisible = false;
        Expand.IsVisible = false;
        Logout.IsVisible = true;
        LabelVal.IsVisible = false;
        Lock.Margin = new Thickness(0, 0, 0, 0);
        Name.IsVisible = true;
        RoleVal.IsVisible = true;
        collapse1.WidthRequest = 65;
        collapse1.HeightRequest = 65;
        Unlock.Margin = new Thickness(0, 0, 0, 20);

        collapse1.Margin = new Thickness(10,5,0,10);
        if (Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.MacCatalyst || Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.macOS){
            Collapse.WidthRequest = 95;
            Collapse.Margin = new Thickness(-10,5,-10,0);
        }


    }
    private void Unlock_Clicked(object sender, EventArgs e)
    {
        _viewModel.FlyoutBehaviors = FlyoutBehavior.Flyout;
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
        Shell.Current.FlyoutIsPresented = false;

        if (Collapse.IsVisible)
        {

            Lock.IsVisible = true;
            Minimize.IsVisible = true;
            Collapse.IsVisible = false;
            Unlock.IsVisible = false;
        } 
        
      
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {

    }
}
