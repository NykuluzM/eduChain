using Microsoft.Maui.Controls;
using eduChain.ViewModels;
using eduChain;
using eduChain.Models;
using Firebase.Auth;

namespace eduChain.Views.ContentPages{

public partial class LoginPage : ContentPage
{

	public LoginPage()
	{
		InitializeComponent();
            var firebaseService = new FirebaseAuthService(); // Initialize FirebaseService if necessary

            // Obtain the FirebaseAuthClient instance from FirebaseService
            var firebaseAuthClient = firebaseService.GetFirebaseAuthClient();

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
            Shell.Current.GoToAsync("//registerPage");
        }
    }
}