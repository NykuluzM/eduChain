
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
		protected override async void OnAppearing(){
			base.OnAppearing();
			await homePageViewModel.LoadProfileAsync(Preferences.Default.Get("firebase_uid", String.Empty));
		}
    }
}