using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Supabase;
using eduChain.Models;
using System.Runtime.Serialization;
using Firebase.Auth;
namespace eduChain.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly FirebaseService firebaseService;
        public event PropertyChangedEventHandler? PropertyChanged;
        private static RegisterViewModel _instance;

        private readonly ISupabaseConnection _supabaseConnection;


                public ICommand TrialCommand { get; }
                public ICommand RegisterCommand { get; }  

        public static RegisterViewModel GetInstance()
        {
              if (_instance == null)
                {
                    _instance = new RegisterViewModel();
                }
            
            return _instance;
        }
        public List<string> GenderOptions { get; private set; }

        public RegisterViewModel()
        {
            GenderOptions = new List<string>
                    {
                        "Male",
                        "Female"
                    };
            firebaseService = FirebaseService.GetInstance();
			var firebaseAuthClient = firebaseService.GetFirebaseAuthClient();
            TrialCommand = new Command(async () => await Trial());
            RegisterCommand = new Command(async () => await Register());
        }


       
         public async Task Trial()
        {
            try
            {
                DatabaseManager.OpenConnection(); // Open the database connection

                // Use the connection for queries, inserts, updates, etc.
                using (var command = DatabaseManager.Connection.CreateCommand())
                {
                        command.CommandText = "SELECT COUNT(*) FROM \"Users\"";
                                    var result = command.ExecuteScalar(); // ExecuteScalar to get a single value

                    await Application.Current.MainPage.DisplayAlert("Users Count", $"There are {result} users", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                DatabaseManager.CloseConnection(); // Close the database connection
            }
        }

        
        private string _firstName = string.Empty; 
        
        private string _lastName = string.Empty;
        private string _gender = string.Empty;
        private DateTime _birthDate ; 

        private string _email = string.Empty;
        private string _password = string.Empty;

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }
        private string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        public string Gender
        {
            get { return _gender; }
            set
            {
                _gender = value;
                OnPropertyChanged(nameof(Gender));
            }
        }
        public DateTime BirthDate
        {
            get { return _birthDate; }
            set
            {
                _birthDate = value;
                OnPropertyChanged(nameof(BirthDate));
            }
        }


        public string Email
        {
                get { return _email; }
                set
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
        }
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
        public void Reset()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Gender = string.Empty;
            BirthDate = DateTime.Now;
            Email = string.Empty;
            Password = string.Empty;
            // Reset other properties as needed
        }

        public async Task Register()
        {
            // Implement the registration logic here
            return;
        }
        // Implement INotifyPropertyChanged members and other properties/methods as needed
    }
}
