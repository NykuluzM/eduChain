using Microsoft.Maui.Controls;
using eduChain.ViewModels;
using eduChain;
using eduChain.Models;
using Firebase.Auth;
using Microsoft.Maui.Storage;
using Mopups.Services;
using CommunityToolkit.Maui.Views;
using UraniumUI.Dialogs;
using UraniumUI.Dialogs.CommunityToolkit;
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
            //InitializeSupabaseAsync();
        }
        private void LoginButton_Clicked(object sender, EventArgs e)
        {
            // Handle the Login button click event
            ((LoginViewModel)BindingContext).Login();

        }
        private async void RegisterButton_Clicked(object sender, EventArgs e)
        {
            NetworkAccess networkAccess = Connectivity.NetworkAccess;
                if(networkAccess == NetworkAccess.None)
                {
                    Application.Current?.MainPage?.DisplayAlert("No Connection", "Lost Internet Connection", "OK");
                    return;
                } 
             var result = await this.DisplayRadioButtonPromptAsync(
            "Pick your Role",
            new [] {"Student", "Organization", "Guardian"});

            if(result == "Student")
            {
                Preferences.Set("Role", "Student");
                var registerPage = new RegisterPage();
                await Shell.Current.Navigation.PushAsync(registerPage);
            }
            else if(result == "Organization")
            {
                Preferences.Set("Role", "Organization");
                var registerOrgPage = new RegisterOrgPage();
                await Shell.Current.Navigation.PushAsync(registerOrgPage);
            }
            else if(result == "Guardian")
            {
                Preferences.Set("Role", "Guardian");
                var registerGuardianPage = new RegisterGuardianPage();
                await Shell.Current.Navigation.PushAsync(registerGuardianPage);
            }
        }
        private void PasswordForgotten(object sender, EventArgs e){
             var forgotPasswordPage = new ForgotPasswordPage();
            Shell.Current.Navigation.PushAsync(forgotPasswordPage);
        }
}
}