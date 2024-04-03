using eduChain.ViewModels;
using eduChain.Models;
using UraniumUI;
using CommunityToolkit.Maui.Views;
using eduChain.Views.Popups;
namespace eduChain.Views.ContentPages
{
    public partial class RegisterPage : ContentPage
    {
		private FirebaseService firebaseService;
		private RegisterViewModel viewModel;
        public RegisterPage()
        {
            InitializeComponent();

            var registerViewModel = RegisterViewModel.GetInstance();

			viewModel = registerViewModel;
            BindingContext = viewModel;
			firebaseService = FirebaseService.GetInstance();
			var firebaseAuthClient = firebaseService.GetFirebaseAuthClient();

        }
        
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
			viewModel.Reset();
        }

        private void check_Format(object sender, EventArgs e)
        {
            this.ShowPopup(new FormatRegPopup());
        }
}
}
