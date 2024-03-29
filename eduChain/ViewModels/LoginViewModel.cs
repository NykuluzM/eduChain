﻿using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Xml;
using CommunityToolkit.Maui.Converters;
using eduChain.Models;
using Firebase.Auth;
using Org.BouncyCastle.Utilities.Collections;


namespace eduChain.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly FirebaseAuthClient _firebaseAuthClient;

        public event PropertyChangedEventHandler? PropertyChanged;
        private static LoginViewModel? _instance;

     

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
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public LoginViewModel(FirebaseAuthClient firebaseAuthClient)
        {   
            _firebaseAuthClient = firebaseAuthClient;
            this.LoginModel = new LoginModel();
            _email = string.Empty;
            LoginCommand = new Command(Login);
        }

            public async void Login()
            {
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
                if (LoginModel != null && isValid)
                {
                    Email = string.Empty;
                    Password = string.Empty;
                // Navigate to another page (e.g., HomePage)
                Shell.Current.GoToAsync("//homePage");
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
          protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler? handler = PropertyChanged;
            if (handler != null)
            {
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            else
            {
                throw new NullReferenceException("PropertyChanged event is not subscribed to.");
            }
        }
    }
}