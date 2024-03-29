using eduChain.ViewModels;
namespace eduChain.Views.ContentPages{
	public partial class HomePage : ContentPage
	{
		public HomePage()
		{
			InitializeComponent();
            BindingContext = new HomePageViewModel();

		}
    }
}