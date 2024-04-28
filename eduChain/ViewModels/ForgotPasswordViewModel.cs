using Microsoft.Maui.Controls;
using eduChain.Models;
using System.ComponentModel;
using Firebase.Auth;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin;
namespace eduChain.ViewModels{

    public class ForgotPasswordViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ICommand SendResetEmailCommand { get; }
        public ICommand NavigateToLoginCommand { get; }
        private static FirebaseService _firebaseService;
          private static ForgotPasswordViewModel _instance;

        public static ForgotPasswordViewModel GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ForgotPasswordViewModel();
            }
            return _instance;
        }
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
            private async void ReadFirebaseAdminSdk(){
            var stream = await FileSystem.OpenAppPackageFileAsync("admin_sdk.json");
            var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(json)
            });
        }
        public ForgotPasswordViewModel(){
            NavigateToLoginCommand = new Command(NavigateToLoginPage);
            SendResetEmailCommand = new Command(SendResetEmail);
            ReadFirebaseAdminSdk();
        }
        
        public async void SendResetEmail()
        {
            
            if (!IsValidEmail(Email))
            {
                if (Application.Current != null && Application.Current.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Invalid Email", "Please enter a valid email address.", "OK");
                }
                
                return;
            }
            try{
                await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.GetUserByEmailAsync(Email);
            } catch (Exception e)
            {
                if (Application.Current != null && Application.Current.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", e.Message, "OK");
                }

                return;
            }
            try{
            _firebaseService = FirebaseService.GetInstance();
            var firebaseAuthClient =  _firebaseService.GetFirebaseAuthClient();
            await firebaseAuthClient.ResetEmailPasswordAsync(Email);
            await Shell.Current.GoToAsync("//loginPage");
            if (Application.Current != null && Application.Current.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Success", $"Make sure you have entered your correct email, Password change is sent to {Email}", "OK");
            }

            return;
            }
            catch(FirebaseAuthException ex){
            if (Application.Current != null && Application.Current.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
                }
                return;
            }
            catch (Exception)
            {
                if (Application.Current != null && Application.Current.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "An unexpected error occurred.", "OK");
                }

                return;
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
       
        private bool IsValidEmail(string email)
        {
            // Define the regex pattern for email validation
            string pattern = @"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$";

            // Perform the regex match
            return Regex.IsMatch(email, pattern);
        }
        private async void NavigateToLoginPage()
        {
            await Shell.Current.Navigation.PopAsync();
        }
    }
}

