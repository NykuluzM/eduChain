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
	}
	

	private async void OnLogoutClicked(object sender, EventArgs e)
	{
		Preferences.Default.Set("IsLoggedIn", false);
		await Shell.Current.GoToAsync("//loginPage");
	}
	private void ExpandCollapse(object sender, TappedEventArgs e)
	{
		if(Expand.IsVisible)
		{
			var animation = new Animation((current) => 
			{
				FlyoutWidth = current;
			}, 75, 275, null);

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
			}, 275, 75, null);

			animation.Commit(this, "collapse", finished: (value, cancelled) =>
			{
				Expand.IsVisible = true;
				Collapse.IsVisible = false;
				Name.IsVisible = false;
				Role.IsVisible = false;
			});	
		}
	}
	 private void ProfileBorder_SizeChanged(object sender, EventArgs e)
        {
            if (sender is Border border)
            {
                // Calculate the size to maintain aspect ratio and fill the circular border
                double radius = border.Height / 2;
                double imageSize = radius * Math.Sqrt(2); // Diagonal of the circle

                // Update the WidthRequest and HeightRequest of the Image
                ProfileImage.WidthRequest = imageSize;
                ProfileImage.HeightRequest = imageSize;
            }
        }
}
