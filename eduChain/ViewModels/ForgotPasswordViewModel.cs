using Microsoft.Maui.Controls;
using eduChain.Models;
using System.ComponentModel;
using Firebase.Auth;
using System.Text.RegularExpressions;
namespace eduChain.ViewModels{

    public class ForgotPasswordViewModel : INotifyPropertyChanged
    {
        private static ForgotPasswordViewModel _instance;
        public Command SendResetEmailCommand { get; }
        private static FirebaseService _firebaseService;

        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
        public static ForgotPasswordViewModel GetInstance()
        {
            {
                if (_instance == null)
                {
                    _instance = new ForgotPasswordViewModel();
                }
                return _instance;
            }
        }
        public ForgotPasswordViewModel(){
            SendResetEmailCommand = new Command(async () => SendResetEmail());
            
        }
        
        public async Task<bool> SendResetEmail()
        {
            if (!IsValidEmail(Email))
            {
                await Application.Current?.MainPage?.DisplayAlert("Invalid Email", "Please enter a valid email address.", "OK");
                
                return false;
            }
            try{
            _firebaseService = FirebaseService.GetInstance();
            var firebaseAuthClient =  _firebaseService.GetFirebaseAuthClient();
            await firebaseAuthClient.ResetEmailPasswordAsync(Email);
            Shell.Current.GoToAsync("//loginPage");
            await Application.Current?.MainPage?.DisplayAlert("Success", $"Make sure you have entered your correct email, Password change is sent to {Email}", "OK");

            return true;
            }
            catch(FirebaseAuthException ex){
                await Application.Current?.MainPage?.DisplayAlert("Error", ex.Message, "OK");
                return false;
            }
            catch (Exception)
            {
                await Application.Current?.MainPage?.DisplayAlert("Error", "An unexpected error occurred.", "OK");

                return false;
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private bool IsValidEmail(string email)
        {
            // Define the regex pattern for email validation
            string pattern = @"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$";

            // Perform the regex match
            return Regex.IsMatch(email, pattern);
        }
    }
}

