using eduChain.ViewModels;
using eduChain.Models;
using UraniumUI;
namespace eduChain.Views.ContentPages
{
    public partial class RegisterPage : ContentPage
    {
		private FirebaseService firebaseService;
		private RegisterViewModel viewModel;
        public RegisterPage()
        {
            InitializeComponent();
			viewModel = RegisterViewModel.GetInstance();
            BindingContext = viewModel;
			firebaseService = FirebaseService.GetInstance();
			var firebaseAuthClient = firebaseService.GetFirebaseAuthClient();

        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
			viewModel.Reset();
        }
    }
}
