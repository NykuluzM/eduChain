using System.ComponentModel;
using System.Windows.Input;
using eduChain.Models;


namespace eduChain.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private static LoginViewModel _instance;
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


        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        private string _password;
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
            LoginCommand = new Command(Login);
        }

        private void Login()
        {
            // Check username and password authenticity
            if (IsValidUser(Username, Password))
            {
                // Navigate to another page (e.g., HomePage)
                Shell.Current.GoToAsync("//homePage");
            }
            else
            {
                // Show error message or handle invalid login
                ShowInvalidCredentialsAlert();
            }
        }
        private void Register(){
            Shell.Current.GoToAsync("//registerPage");
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

        private void NavigateToHomePage()
        {
            // Implement navigation logic to navigate to the home page
            // For example, using a navigation service or Shell navigation
        }
        
        private void ShowInvalidCredentialsAlert()
        {
            Application.Current.MainPage.DisplayAlert("Error", "Invalid username or password", "OK");
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