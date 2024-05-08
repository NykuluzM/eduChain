using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Supabase;
using eduChain.Models;
using System.Runtime.Serialization;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Npgsql;
using NBitcoin.Secp256k1;

using System.Text.RegularExpressions;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
namespace eduChain.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private FirebaseAuthClient firebaseAuthClient;
        public event PropertyChangedEventHandler? PropertyChanged;
        private static RegisterViewModel _instance;



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
        public static void ResetInstance()
        {
            _instance = null;
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

                    await Shell.Current.DisplayAlert("Users Count", $"There are {result} users", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                await DatabaseManager.CloseConnectionAsync(); // Close the database connection
            }
        }

        private string _displayName = string.Empty;
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }
        private string _firstName = string.Empty;

        private string _lastName = string.Empty;
        private string _gender = "Male";
        private DateTime _birthDate = new DateTime(2000, 1, 1);

        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _orgName = string.Empty;

        private string _type = string.Empty;

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
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }
        public string OrgName
        {
            get { return _orgName; }
            set
            {
                _orgName = value;
                OnPropertyChanged(nameof(OrgName));
            }
        }
        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
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
                var error = false;
                switch (Preferences.Default.Get("Role", ""))
                {
                    case "Student":
                        if(DisplayName.Length < 6){
                            await Shell.Current.DisplayAlert("Error", "Display name must be at least 6 characters long", "OK");
                            return;
                        }
                        if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || string.IsNullOrEmpty(Gender) || (BirthDate > DateTime.Now.AddYears(-4)))
                        {
                            await Shell.Current.DisplayAlert("Error", "Please fill in all fields and format correctly", "OK");
                            return;
                        }
                        string fnamePattern1 = @"^(?:[ A-Z\d][a-z\d]*(?:\s*[A-Z\d][a-z\d]*)*|)$";
                        string fnamePattern2 = @"^(?![ ])[A-Za-z ]*$";
                        string lnamePattern = @"^(?:[A-Z][a-z]*|)$";

                        // Validate FirstName and LastName against the regex pattern
                        if (!Regex.IsMatch(FirstName, fnamePattern1) || (!Regex.IsMatch(FirstName, fnamePattern2)) || !Regex.IsMatch(LastName, lnamePattern))
                        {
                            await Shell.Current.DisplayAlert("Error", "Please enter a valid first and last name", "OK");
                            error = true;
                        }
                        string emailPattern = @"^(?:\s*|(?:[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}))$";
                        if (!Regex.IsMatch(Email, emailPattern))
                        {
                            await Shell.Current.DisplayAlert("Error", "Please enter a valid email address", "OK");
                            error = true;
                        }
                        string passwordPattern = @"^(?:[A-Za-z][A-Za-z0-9_*#]*(?:_[A-Za-z0-9_*#]+)*)?$";
                        if (string.IsNullOrEmpty(Password) || Password.Length < 6 || !Regex.IsMatch(Password, passwordPattern))
                        {
                            await Shell.Current.DisplayAlert("Error", "Please enter a valid password (at least 6 characters long) with the specified pattern", "OK");
                            error = true;
                        }
                        if (error)
                        {
                            return;
                        }


                        break;
                    case "Organization":
                        if (string.IsNullOrEmpty(OrgName) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Type))
                        {
                            await Shell.Current.DisplayAlert("Error", "Please fill in all fields", "OK");
                            return;
                        }
                        string emailPatternOrg = @"^(?:\s*|(?:[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}))$";

                        if (!Regex.IsMatch(Email, emailPatternOrg))
                        {
                            await Shell.Current.DisplayAlert("Error", "Please enter a valid email address", "OK");
                            error = true;
                        }
                        string passwordPatternOrg = @"^(?:[A-Za-z][A-Za-z0-9_*#]*(?:_[A-Za-z0-9_*#]+)*)?$";
                        if (string.IsNullOrEmpty(Password) || Password.Length < 6 || !Regex.IsMatch(Password, passwordPatternOrg))
                        {
                            await Shell.Current.DisplayAlert("Error", "Please enter a valid password (at least 6 characters long) with the specified pattern", "OK");
                            error = true;
                        }
                        if (error)
                        {
                            return;
                        }
                        break;
                }
                // Perform user registration with Firebase and get the auth result
                var authResult = await firebaseAuthClient.CreateUserWithEmailAndPasswordAsync(Email, Password);
                
                // Check if registration was successful
                if (authResult.User != null)
                {
                    // Extract the user UID from the auth result
                    string uid = authResult.User.Uid;
                    // Save user details to the database
                    var res = await SaveUserDetailsAsync(uid, Email);
                    if (res == 0)
                    {
                        //await firebaseAuthClient.DeleteUserAsync(uid);
                    }

                    await Shell.Current.GoToAsync("//loginPage");
                }
            }
            catch (FirebaseAuthException ex)
            {
                // Handle Firebase authentication exceptions first
                if (ex.Reason == AuthErrorReason.EmailExists)
                {
                    await Shell.Current.DisplayAlert("Error", "Email already in use", "OK");
                }
                else if (ex.Reason == AuthErrorReason.WeakPassword)
                {
                    await Shell.Current.DisplayAlert("Error", "Password is too weak", "OK");
                }
                else if (ex.Reason == AuthErrorReason.Unknown)
                {
                    await Shell.Current.DisplayAlert("Error", "An unknown error occurred", "OK");
                }
                else if (ex.Reason == AuthErrorReason.UserDisabled)
                {
                    await Shell.Current.DisplayAlert("Error", "User is disabled", "OK");
                }

            }
            catch (Exception ex)
            {
                // Handle other exceptions (if any)
                await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
        public async Task<int> SaveUserDetailsAsync(string uid, string email)
        {
            try
            {
                await DatabaseManager.OpenConnectionAsync();
                using (var command = DatabaseManager.Connection.CreateCommand())
                {
                    var parameters = command.Parameters;
                    switch (Preferences.Default.Get("Role", ""))
                    {
                        case "Student":
                            if (parameters is NpgsqlParameterCollection pgParameters)
                            {
                                pgParameters.AddWithValue("@email", email);
                                pgParameters.AddWithValue("@display_name", DisplayName);
                                pgParameters.AddWithValue("@firebase_id", uid);
                                pgParameters.AddWithValue("@first_name", FirstName);
                                pgParameters.AddWithValue("@last_name", LastName);
                                pgParameters.AddWithValue("@gender", Gender);
                                pgParameters.AddWithValue("@birth_date", BirthDate);
                                pgParameters.AddWithValue("@role", "Student");

                            }

                            command.CommandText = @"
                            WITH inserted_user AS (
                                INSERT INTO ""Users"" (""firebase_id"", ""display_name"",""email"",""role"") VALUES (@firebase_id,@display_name,@email,@role) RETURNING ""firebase_id""
                            )
                            INSERT INTO ""Students"" (""user_firebase_id"", ""first_name"", ""last_name"", ""gender"", ""birth_date"") 
                            SELECT ""firebase_id"", @first_name, @last_name, @gender, @birth_date FROM inserted_user";

                            await command.ExecuteNonQueryAsync();
                            Email = string.Empty;
                            Password = string.Empty;
                            FirstName = string.Empty;
                            LastName = string.Empty;
                            BirthDate = DateTime.Now.AddYears(-4);
                            await Shell.Current.DisplayAlert("Success", "User registered successfully", "OK");

                            break;
                        case "Organization":
                            if (parameters is NpgsqlParameterCollection pgParametersOrg)
                            {
                                pgParametersOrg.AddWithValue("@display_name", DisplayName);
                                pgParametersOrg.AddWithValue("@email", email);
                                pgParametersOrg.AddWithValue("@firebase_id", uid);
                                pgParametersOrg.AddWithValue("@name", OrgName);
                                pgParametersOrg.AddWithValue("@type", Type);
                                pgParametersOrg.AddWithValue("@role", "Organization");
                            }

                            command.CommandText = @"
                            WITH inserted_user AS (
                                INSERT INTO ""Users"" (""firebase_id"",""display_name"",""email"", ""role"") VALUES (@firebase_id,@display_name, @email, @role) RETURNING ""firebase_id""
                            )
                            INSERT INTO ""Organizations"" (""user_firebase_id"", ""name"", ""type"") 
                            SELECT ""firebase_id"", @name, @type FROM inserted_user";
                            await command.ExecuteNonQueryAsync();
                            await Application.Current.MainPage.DisplayAlert("Success", "User registered successfully", "OK");

                            break;
                        case "Guardian":

                            break;

                    }
                    // Cast the parameters to NpgsqlParameterCollection to use AddWithValue
                    return 1;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                return 0;
            }
            finally
            {
                await DatabaseManager.CloseConnectionAsync();
            }
        }

    }
}