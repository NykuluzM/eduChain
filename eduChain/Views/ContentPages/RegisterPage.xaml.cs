using eduChain.ViewModels;
using eduChain.Models;
using UraniumUI;
using CommunityToolkit.Maui.Views;
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

        }
}
