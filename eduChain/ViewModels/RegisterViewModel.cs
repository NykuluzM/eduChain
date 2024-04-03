using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Supabase;
using eduChain.Models;  

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

        public RegisterViewModel()
        {

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
                        command.CommandText = "SELECT COUNT(*) FROM students";
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
