
namespace eduChain.Views.ContentPages{
	public partial class HomePage : ContentPage
	{
		private HomePageViewModel homePageViewModel;
		public HomePage()
		{
			InitializeComponent();
			homePageViewModel = new HomePageViewModel();
            BindingContext = homePageViewModel;
			
		}
    }
}