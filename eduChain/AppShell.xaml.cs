namespace eduChain;
using eduChain.Views.ContentPages;
using eduChain.Views.ContentPages.ProfileViews;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Microsoft.Maui.Storage;
using eduChain.Models;
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
	}
	private async void ProfileTapped(object sender, EventArgs e)
	{
		if(Preferences.Default.Get("Role", String.Empty) == "Student"){
			await Shell.Current.Navigation.PushAsync(new StudentProfilePage(),false);
		}
		else if(Preferences.Default.Get("Role", String.Empty) == "Organization"){
			await Shell.Current.Navigation.PushAsync(new OrganizationProfilePage(),false);
		}
		else if(Preferences.Default.Get("Role", String.Empty) == "Guardian"){
			await Shell.Current.Navigation.PushAsync(new GuardianProfilePage());
		}
		
	}
	private void ExitFlyout(object sender, EventArgs e)
	{
        Shell.Current.FlyoutIsPresented = false;
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
