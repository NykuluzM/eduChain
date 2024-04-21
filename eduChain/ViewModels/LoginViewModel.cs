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
namespace eduChain.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        
        private readonly FirebaseAuthClient _firebaseAuthClient;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {   
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public LoginModel LoginModel { get; set; }
        public ICommand LoginCommand { get; }


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
                if(_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        private bool hasNavigated = false;
      
        public LoginViewModel(FirebaseAuthClient firebaseAuthClient)
        {
            _firebaseAuthClient = firebaseAuthClient;
            this.LoginModel = new LoginModel();
            LoginCommand = new Command(Login);

            // Call the method for navigation based on authentication state
        }

        public bool KeepLoggedIn { get; set; } // Property to store checkbox state


            public async void Login()
            {
                NetworkAccess networkAccess = Connectivity.NetworkAccess;
                if(networkAccess == NetworkAccess.None)
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
                    if(KeepLoggedIn)
                    {
                        Preferences.Default.Set("email", Email);
                        Preferences.Default.Set("password", Password);
                        Preferences.Default.Set("IsLoggedIn", true);
                            Application.Current?.MainPage?.DisplayAlert("Error", "saved", "OK");
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
            catch (FirebaseAuthException ex)
            {
                return false;
            }
        }

        private void ShowInvalidCredentialsAlert(string context)
        {
            if(context == "inputcomplete")
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
        
        
    }
}