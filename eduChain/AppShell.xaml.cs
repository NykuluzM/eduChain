namespace eduChain;
using eduChain.Views.ContentPages;
using eduChain.Views.ContentPages.ProfileViews;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.Maui.Storage;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using eduChain.Models;
using Nethereum.Model;
using System.Drawing;

public partial class AppShell : Shell
{
    bool repeat = true;
    bool navPressed = false;
    bool norepeat = true;
    bool isCollapsed;
    AppShellViewModel _viewModel;

    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(Routes.RegisterPage, typeof(RegisterPage));

        Routing.RegisterRoute("forgotPasswordPage", typeof(ForgotPasswordPage));
        _viewModel = new AppShellViewModel();
        this.BindingContext = _viewModel;
        if(DeviceInfo.Platform == DevicePlatform.Android){
            var tapGestureRecognizer = new TapGestureRecognizer();
            pTap.GestureRecognizers.Add(tapGestureRecognizer);
            tapGestureRecognizer.Tapped += ProfileTapped;
        } else {
            PointerGestureRecognizer pointerGestureRecognizer = new PointerGestureRecognizer();
            pointerGestureRecognizer.PointerEntered += (s, e) =>
            {
                HoverEffect(this, e);
            };
            pointerGestureRecognizer.PointerExited += (s, e) =>
            {
                HoverEffectOut(this, e);
                // Handle the pointer exited event
            };
            pTap.GestureRecognizers.Add(pointerGestureRecognizer);
        }
    }
    public void TriggerLayout(string layout)
    {
        Shell.Current.FlyoutBehavior = _viewModel.FlyoutBehaviors;

        if (layout == "collapsed")
        {
            LockFlyout(this, new EventArgs());

            Collapse_Clicked(this, new EventArgs());


        }
        else if (layout == "collapsed_locked")
        {
            
        }
        else
        {
            Shell.Current.DisplayAlert("Layout", "Expanded", "OK");
        }
    }
    private async void ProfileTapped(object sender, EventArgs e)
    {
        navPressed = true;
        //fly1.On<WinUI>.SetIsEnabled(false);
        if (Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.Android || Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.iOS)
        {
            fly1.IsEnabled = false;
            fly2.IsEnabled = false;
            fly3.IsEnabled = false;
            pTap.IsEnabled = false;
        }

        if (!repeat)
        {
            return;
        }
        switch (Preferences.Default.Get("Role", String.Empty))
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
                    try
                    {
                        await Shell.Current.Navigation.PushAsync(new StudentProfilePage(), false);
                        repeat = true;
                    }
                    catch (Exception ex)
                    {
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
    private async void HoverEffect(object sender, EventArgs e){
        if(isCollapsed){
            tapper.Text = "";
        } else {
            tapper.Text = "Check Profile";
        }
        collapse1.IsVisible = false;
        RoleVal.IsVisible = false;
        Name.IsVisible = false;
        Labels.IsVisible = false;
        if(DeviceInfo.Platform == DevicePlatform.MacCatalyst)
        {
            tapper.IsVisible = true;
        } else {
            await tapper.FadeTo(1, 700);

        }

        b1.Opacity = 0.25;
        b2.Opacity = 0.25;
        b3.Opacity = 0.25;

    }
    private async void HoverEffectOut(object sender, EventArgs e){
        collapse1.IsVisible = true;
        if(isCollapsed){
            RoleVal.IsVisible = false;
            Name.IsVisible = false;
            Labels.IsVisible = true;
        } else {
            RoleVal.IsVisible = true;
            Name.IsVisible = true;
            Labels.IsVisible = false;
        }
         if(DeviceInfo.Platform == DevicePlatform.MacCatalyst)
        {
            tapper.IsVisible = false;
        } else {
            await tapper.FadeTo(0, 700);
        }
        b1.Opacity = 1;
        b2.Opacity = 1;
        b3.Opacity = 1;
       
    }

    private void AboutDevTapped(object sender, EventArgs e){
        //Shell.Current.Navigation.PushAsync(new AboutDevPage());
    }
    private void AboutDevHover(object sender, EventArgs e){
    }
    private void AboutDevHoverOut(object sender, EventArgs e){
    }
    private void ExitFlyout(object sender, EventArgs e)
    {
        _viewModel.FlyoutBehaviors = FlyoutBehavior.Flyout;
      

    }
    private void LockFlyout(object sender, EventArgs e)
    {
        _viewModel.FlyoutBehaviors = FlyoutBehavior.Locked;
        Lock.IsVisible = false;
       
        Unlock.IsVisible = true;
        Collapse.IsVisible = true;
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
        isCollapsed = true;
        var size1 = 270;
        var size2 = 80;
        if(DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
        {
            size1 = 270;
            size2 = 50;
        } else if (DeviceInfo.Platform == DevicePlatform.MacCatalyst){
            pTap.Padding = new Thickness(0, 0, 0, 0);
        }
        var animation = new Animation((current) => {
            FlyoutWidth = current;
        },size1,size2, null);
        animation.Commit(this, "Collapse", finished: (value, cancelled) =>
        {
            Expand.IsVisible = true;
            Collapse.IsVisible = false;
        });
        
       
            Unlock.IsVisible = false;
            Lock.IsVisible = false;
       
        Expand.RotateTo(90, 500, new Easing(t => t));
        Expand.IsVisible = true;
        Logout.IsVisible = false;
        Name.IsVisible = false;
        RoleVal.IsVisible = false;
        collapse1.WidthRequest = 35;
        collapse1.HeightRequest = 35;
        Expand.Margin = new Thickness(0, 50, 0, 25);
        LabelVal.IsVisible = true;
        LabelLog.IsVisible = true;
        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            collapse1.WidthRequest = 45;
            collapse1.HeightRequest = 45;
            collapse1.Margin = new Thickness(15, 5, 10, 15);
           
        } else if (DeviceInfo.Platform == DevicePlatform.MacCatalyst)
        {
            collapse1.WidthRequest = 45;
            collapse1.HeightRequest = 45;
            collapse1.Margin = new Thickness(-4.8, 15, 0, 0);
            collapse1.VerticalOptions = LayoutOptions.Start;
            LabelVal.FontSize = 10;
            LabelVal.TextColor = Colors.Black;
            LabelVal.HorizontalOptions = LayoutOptions.Center;
        }
        else
        {
            collapse1.HorizontalOptions = LayoutOptions.Center;
            collapse1.VerticalOptions = LayoutOptions.Center;
            collapse1.Margin = new Thickness(5.5, 0, 0, 0);

            LabelVal.FontSize = 8;
            LabelVal.HorizontalOptions = LayoutOptions.Center;
            LabelVal.TextColor = Colors.Black;


        }


    }
   
   
    private async void Expand_Clicked(object seder, EventArgs e)
    {
        isCollapsed = false;
        if(!norepeat)
        {
            return;
        }
        var size1 = 270;
        var size2 = 50;
        if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS)
        {
            size1 = 270;
            size2 = 60;
            pTap.Padding = new Thickness(0, 0, 0, 0);
        }
        var animation = new Animation((current) => {
            FlyoutWidth = current;
        }, size2, size1, null);
        animation.Commit(this, "Expand", finished: async (value, cancelled) =>
        {
            Expand.IsVisible = false;
            Collapse.IsVisible = true;

            await Task.Delay(200);
            if (navPressed && norepeat)
            {
                norepeat = false;
                await UpdateGrid();
                norepeat = true;
                navPressed = false;
            }
        });
        Collapse.IsVisible = true;
      
        Unlock.IsVisible = true;
        Lock.IsVisible = false;
       


        Logout.IsVisible = true;
        LabelVal.IsVisible = false;
        LabelLog.IsVisible = false;
        Lock.Margin = new Thickness(0, 0, 0, 0);
        Name.IsVisible = true;
        RoleVal.IsVisible = true;
        collapse1.WidthRequest = 65;
        collapse1.HeightRequest = 65;
        Unlock.Margin = new Thickness(0, 0, 0, 20);

        collapse1.Margin = new Thickness(20, 5, 0, 10);
        if (Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.MacCatalyst || Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.macOS)
        {
            Collapse.WidthRequest = 95;
            Collapse.Margin = new Thickness(-10, 5, -10, 0);
        }
       
      
    }
    private async Task UpdateGrid()
    {
        navPressed = false;
        //Assumes already that flyout is locked and collapsed to reproduce the layout issues
        await UpdateVal();
        await Task.Delay(50);
        Unlock_Clicked(this, new EventArgs());
        await Task.Delay(50);

        LockFlyout(this, new EventArgs());
    }
    private async Task UpdateVal()
    {
        Expand_Clicked(this, new EventArgs());

    }
   
    private  void Unlock_Clicked(object sender, EventArgs e)
    {

        Collapse.IsVisible = false;
        Lock.IsVisible = true;
        Unlock.IsVisible = false;
       
        _viewModel.FlyoutBehaviors = FlyoutBehavior.Flyout; 
    }
  
}