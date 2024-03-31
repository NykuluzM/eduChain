using Microsoft.Maui.Controls;
using eduChain.ViewModels;
using eduChain;
using eduChain.Models;
using Firebase.Auth;
using Microsoft.Maui.Storage;
namespace eduChain.Views.ContentPages{
public partial class LoginPage : ContentPage
{
        private readonly FirebaseService _firebaseService;

        public LoginPage()
	    {
            Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
            
		    InitializeComponent();
            _firebaseService = FirebaseService.GetInstance();

            var firebaseAuthClient = _firebaseService.GetFirebaseAuthClient();

                // Create an instance of LoginViewModel with FirebaseAuthClient instance
            var loginViewModel = new LoginViewModel(firebaseAuthClient);

                // Set the BindingContext of the page to the LoginViewModel instance
            BindingContext = loginViewModel;
    
        }


        private void TextFieldPasswordShowHideAttachment_LayoutChanged(object sender, EventArgs e)
        {

        }
        private void LoginButton_Clicked(object sender, EventArgs e)
        {
            // Handle the Login button click event
            ((LoginViewModel)BindingContext).Login();

        }
        private void RegisterButton_Clicked(object sender, EventArgs e)
        {
            var registerPage = new RegisterPage();
            Shell.Current.Navigation.PushAsync(registerPage);
        }
    }
}