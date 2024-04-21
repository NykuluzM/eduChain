namespace eduChain;
using eduChain.Models;
using eduChain.Services;
using Npgsql;
using Firebase.Auth;
using System.Windows.Input;

public class RegisterOrgViewModel : ViewModelBase 
{        
    public ICommand RegisterCommand { get; }  
    private readonly FirebaseAuthClient firebaseAuthClient;
    private string _email;
    private string _password;
    private string _name;
    public RegisterOrgViewModel()
    {
        firebaseAuthClient = FirebaseAuthService.GetInstance().GetFirebaseAuthClient();
        RegisterCommand = new Command(async () => await Register());
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
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
    }

    public string Name
    {
        get { return _name; }
        set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
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
                pgParameters.AddWithValue("@name", Name);


            }

            command.CommandText = "INSERT INTO \"Users\" (\"firebase_id\", \"name\", \"role\" ) VALUES (@firebase_id, @name, @role)";
            await command.ExecuteNonQueryAsync();
            Name = string.Empty;
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
