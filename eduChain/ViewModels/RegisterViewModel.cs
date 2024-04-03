using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Supabase;
using eduChain.Models;
using System.Runtime.Serialization;
namespace eduChain.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private static RegisterViewModel _instance;

        private readonly ISupabaseConnection _supabaseConnection;


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
            TrialCommand = new Command(async () => await Trial());
        }

        public ICommand TrialCommand { get; }

       
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

                
        private string _gender = string.Empty;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
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
    private DateTime _birthDate ; 
public DateTime BirthDate
{
    get { return _birthDate; }
    set
    {
        _birthDate = value;
        OnPropertyChanged(nameof(BirthDate));
    }
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
            Email = string.Empty;
            Password = string.Empty;
            // Reset other properties as needed
        }
        // Implement INotifyPropertyChanged members and other properties/methods as needed
    }
}
