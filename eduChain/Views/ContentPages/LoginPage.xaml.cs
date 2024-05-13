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

        public LoginPage()
	    {
            Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
            
		    InitializeComponent();
            var _firebaseService = FirebaseService.GetInstance();

            var firebaseAuthClient = _firebaseService.GetFirebaseAuthClient();

                // Create an instance of LoginViewModel with FirebaseAuthClient instance
            var loginViewModel = new LoginViewModel(firebaseAuthClient);

                // Set the BindingContext of the page to the LoginViewModel instance
            BindingContext = loginViewModel;
            //InitializeSupabaseAsync();
        }
       
    
}
}