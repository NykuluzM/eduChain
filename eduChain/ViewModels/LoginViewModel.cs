using System.ComponentModel;
using System.Windows.Input;
using System.Xml;
using CommunityToolkit.Maui.Converters;
using eduChain.Models;
using Org.BouncyCastle.Utilities.Collections;


namespace eduChain.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private static LoginViewModel? _instance;
        public static LoginViewModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LoginViewModel();
                return _instance;
            }
            
        }

        public LoginModel LoginModel { get; set; }
        public ICommand LoginCommand { get; }


        private string _username = string.Empty;
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
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

        public LoginViewModel()
        {   
            this.LoginModel = new LoginModel();
            _username = string.Empty;
            LoginCommand = new Command(Login);
        }

        public void Login()
        {
            if (string.IsNullOrWhiteSpace(Username) && string.IsNullOrWhiteSpace(Password))
            {
                ShowInvalidCredentialsAlert("inputnothing");
                return;
            }

            if (string.IsNullOrWhiteSpace(Username))
            {
                ShowInvalidCredentialsAlert("inputnousername");
                return;
            }
            if (string.IsNullOrEmpty(Password))
            {
                ShowInvalidCredentialsAlert("inputnopassword");
                return;
            }
           
            // Check username and password authenticity
            if (LoginModel != null && IsValidUser(Username, Password))
            {
                // Navigate to another page (e.g., HomePage)
                Shell.Current.GoToAsync("//homePage");
            }
            else
            {
                // Show error message or handle invalid login
                ShowInvalidCredentialsAlert("inputcomplete");
            }
        }

        private bool IsValidUser(string username, string password)
        {
            //var userService = new UserService(); // Assuming UserService is a class to interact with the database
            //var user = userService.GetUserByUsername(username);

            //if (user != null && user.Password == password)
            if(username == "admin" && password == "admin")
            {
                return true; // Valid username and password
            }
            else
            {
                return false; // Invalid credentials
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