using eduChain.ViewModels;
namespace eduChain.Views.ContentPages{
	public partial class HomePage : ContentPage
	{
		public HomePage()
		{
			Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
			InitializeComponent();
			var homePageViewModel = new HomePageViewModel();
            BindingContext = homePageViewModel;

		}
    }
}