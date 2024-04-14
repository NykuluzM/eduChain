using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Supabase;
using eduChain.Models;
using System.Runtime.Serialization;
using Firebase.Auth;
using Npgsql;
namespace eduChain.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly FirebaseAuthClient firebaseAuthClient;
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
            var firebaseService = FirebaseService.GetInstance();
			firebaseAuthClient = firebaseService.GetFirebaseAuthClient();
            TrialCommand = new Command(async () => await Trial());
            RegisterCommand = new Command(async () => await Register());
        }


       
         public async Task Trial()
        {
            try
            {
                await DatabaseManager.OpenConnectionAsync(); // Open the database connection

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
                await DatabaseManager.CloseConnectionAsync(); // Close the database connection
            }
        }

        
        private string _firstName = string.Empty; 
        
        private string _lastName = string.Empty;
        private string _gender = string.Empty;
        private DateTime _birthDate = DateTime.Now; 

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
        public string LastName
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
      
        public async Task Register()
        {
            try
            {
                // Perform user registration with Firebase and get the auth result
                var authResult = await firebaseAuthClient.CreateUserWithEmailAndPasswordAsync(Email, Password);

                // Check if registration was successful
                if (authResult != null)
                {
                    // Extract the user UID from the auth result
                    string uid = authResult.User.Uid;

                    // Save user details to the database
                    await SaveUserDetailsAsync(uid);   
                    
                  
                    await Shell.Current.GoToAsync("//loginPage");                 
                }
            }
            catch (FirebaseAuthException ex)
            {
                // Handle Firebase authentication exceptions first
                await Application.Current.MainPage.DisplayAlert("Error", $"Firebase error: {ex.Message}", "OK");
            }
            catch (Exception ex)
            {
                // Handle other exceptions (if any)
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
public async Task SaveUserDetailsAsync(string uid)
{
    try
    {
        await DatabaseManager.OpenConnectionAsync();
        using (var command = DatabaseManager.Connection.CreateCommand())
        {
            var parameters = command.Parameters;

            // Cast the parameters to NpgsqlParameterCollection to use AddWithValue
            if (parameters is NpgsqlParameterCollection pgParameters)
            {
                pgParameters.AddWithValue("@firebase_id", uid);
                pgParameters.AddWithValue("@first_name", FirstName);
                pgParameters.AddWithValue("@last_name", LastName);
                pgParameters.AddWithValue("@gender", Gender);
                pgParameters.AddWithValue("@birth_date", BirthDate);
                pgParameters.AddWithValue("@role", "Student");

            }

            command.CommandText = "INSERT INTO \"Users\" (\"firebase_id\", \"first_name\", \"last_name\", \"gender\", \"birth_date\", \"role\" ) VALUES (@firebase_id, @first_name, @last_name, @gender, @birth_date, @role)";
            await command.ExecuteNonQueryAsync();
            Email = string.Empty;
            Password = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            BirthDate = DateTime.Now;
            await Application.Current.MainPage.DisplayAlert("Success", "User registered successfully", "OK");
        }
    }
    catch (Exception ex)
    {
        await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
    }
    finally
    {
        await DatabaseManager.CloseConnectionAsync();
    }
}

    }
}
