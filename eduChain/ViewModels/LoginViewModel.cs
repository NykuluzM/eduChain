using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Xml;
using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Core.Views;
using eduChain.Models;
using Firebase.Auth;
using Org.BouncyCastle.Utilities.Collections;
using Microsoft.Maui.Storage;
using eduChain.Views.ContentPages;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Alerts;
namespace eduChain.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {

        private readonly FirebaseAuthClient _firebaseAuthClient;

        public ICommand LoginCommand { get; }
        public ICommand ForgottenCommand { get; }
        public ICommand RegisterCommand { get; }

        private string _email = string.Empty;
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
        private string _password = string.Empty;
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }


        public LoginViewModel(FirebaseAuthClient firebaseAuthClient)
        {
            _firebaseAuthClient = firebaseAuthClient;
            LoginCommand = new Command(async () => await Login());
            ForgottenCommand = new Command(async () => await Forgotten());
            RegisterCommand = new Command(async () => await Register());
            // Call the method for navigation based on authentication state
        }

        public bool KeepLoggedIn { get; set; } 


        public async Task Login()
        {
            NetworkAccess networkAccess = Connectivity.NetworkAccess;
            if (networkAccess == NetworkAccess.None)
            {
                Application.Current?.MainPage?.DisplayAlert("No Connection", "Lost Internet Connection", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(Password))
            {
                ShowInvalidCredentialsAlert("inputnothing");
                return;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                ShowInvalidCredentialsAlert("inputnousername");
                return;
            }
            if (string.IsNullOrEmpty(Password))
            {
                ShowInvalidCredentialsAlert("inputnopassword");
                return;
            }
            bool isValid = await IsValidUser(Email, Password);


            // Check username and password authenticity


            if (isValid)
            {
                if (KeepLoggedIn)
                {
                    
                    Preferences.Default.Set("email", Email);
                    Preferences.Default.Set("password", Password);
                    Preferences.Default.Set("IsLoggedIn", true);
                    var cancellationTokenSource = new CancellationTokenSource();
                    var text = "Saved Login";
                    var duration = ToastDuration.Long;
                    var fontSize = 14;
                    var toast = Toast.Make(text, duration, fontSize);
                    await toast.Show(cancellationTokenSource.Token);
                    
                }
                Password = string.Empty;
                Preferences.Default.Set("email", Email);
                Email = string.Empty;
                // Navigate to the home page
                //var homePage = new HomePage();
                Preferences.Default.Set("isloaded", "false");
                await Shell.Current.GoToAsync($"//homePage?forceNewInstance=true");

            }
            else
            {
                // Show error message or handle invalid login
                ShowInvalidCredentialsAlert("inputcomplete");
            }
        }

        private async Task<bool> IsValidUser(string email, string password)
        {
            try
            {
                // Authenticate user with email and password using Firebase Authentication
                var userCredential = await _firebaseAuthClient.SignInWithEmailAndPasswordAsync(email, password);
                Preferences.Default.Set("firebase_uid", userCredential.User.Uid);

                // Check if user authentication was successful
                if (userCredential != null && userCredential.User != null)
                {
                    return true; // Valid email and password
                }
                else
                {
                    return false; // Invalid email or password
                }
            }
            catch (FirebaseAuthException)
            {
                return false;
            }
        }

        private void ShowInvalidCredentialsAlert(string context)
        {
            if (context == "inputcomplete")
            {
                Application.Current?.MainPage?.DisplayAlert("Error", "Invalid username or password", "OK");
            }
            else if (context == "inputnousername")
            {
                Application.Current?.MainPage?.DisplayAlert("Error", "Please provide a username", "OK");
            }
            else if (context == "inputnopassword")
            {
                Application.Current?.MainPage?.DisplayAlert("Error", "Please provide a password", "OK");
            }
            else if (context == "inputnothing")
            {
                Application.Current?.MainPage?.DisplayAlert("Error", "Please provide a username and password", "OK");
            }
        }

        private async Task Register()
        {
            NetworkAccess networkAccess = Connectivity.NetworkAccess;
            if (networkAccess == NetworkAccess.None)
            {
                Application.Current?.MainPage?.DisplayAlert("No Connection", "Lost Internet Connection", "OK");
                return;
            }
            var result = await Shell.Current.DisplayActionSheet("Role?", "Student", "Organization");

            if (result == "Student")
            {
                Preferences.Set("Role", "Student");
                var registerPage = new RegisterStudPage();
                await Shell.Current.Navigation.PushAsync(registerPage);
            }
            else if (result == "Organization")
            {
                Preferences.Set("Role", "Organization");
                var registerOrgPage = new RegisterOrgPage();
                await Shell.Current.Navigation.PushAsync(registerOrgPage);
            }

        }
        private async Task Forgotten()
        {
            var forgotPasswordPage = new ForgotPasswordPage();
            await Shell.Current.Navigation.PushAsync(forgotPasswordPage);
        }
    }
}