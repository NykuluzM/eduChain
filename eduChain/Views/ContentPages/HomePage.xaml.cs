using eduChain.ViewModels;
namespace eduChain.Views.ContentPages{
	public partial class HomePage : ContentPage
	{
		public HomePage()
		{
			InitializeComponent();
		}
        private void LogoutButton_Clicked(object sender, EventArgs e)
        {
            // Handle the Login button click event

            Shell.Current.GoToAsync("//loginPage");

        }
    }
}