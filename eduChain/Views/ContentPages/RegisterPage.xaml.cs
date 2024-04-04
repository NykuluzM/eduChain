using eduChain.ViewModels;
using eduChain.Models;
using UraniumUI;
using CommunityToolkit.Maui.Views;
using eduChain.Views.Popups;
namespace eduChain.Views.ContentPages
{
    public partial class RegisterPage : ContentPage
    {
		private RegisterViewModel viewModel;
        public RegisterPage()
        {
            InitializeComponent();

            var registerViewModel = RegisterViewModel.GetInstance();

			viewModel = registerViewModel;
            BindingContext = viewModel;
			

        }

        private void check_Format(object sender, EventArgs e)
        {
            this.ShowPopup(new FormatRegPopup());
        }
}
}
