using eduChain.Models;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace eduChain.Services
{
    public class MyProfileService
    {
        private static MyProfileService instance;

        private readonly ISupabaseConnection _supabaseConnection;

        public MyProfileService(ISupabaseConnection supabaseConnection)
        {
            _supabaseConnection = supabaseConnection;
        }
          public static MyProfileService Instance
        {
            get
            {
                if (instance == null)
                {
                    // Initialize the instance if it's null
                    instance = new MyProfileService(IPlatformApplication.Current.Services.GetRequiredService<ISupabaseConnection>()); // Add the required service
                }
                return instance;
            }
        }
        public async Task<MyProfileModel> GetUserProfileAsync(string uid, MyProfileModel profile)
        {
            try
            {
                await DatabaseManager.OpenConnectionAsync();
                using (var command = DatabaseManager.Connection.CreateCommand())
                {
                    command.CommandTimeout = 5; // Example: 30 seconds

                    var parameters = command.Parameters;

                    if (parameters is NpgsqlParameterCollection pgParameters)
                    {
                        pgParameters.AddWithValue("@firebase_id", uid);
                    }

                    command.CommandText = "SELECT * FROM \"Users\" WHERE \"firebase_id\" = @firebase_id";
                       const int maxRetries = 3;
                    int retries = 0;

                    while (retries < maxRetries)
                    {
                        try{
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                            
                                profile.Email = Preferences.Default.Get("email", String.Empty);
                                profile.FirstName = reader["first_name"] is DBNull ? null : reader["first_name"].ToString();
                                profile.LastName = reader["last_name"] is DBNull ? null : reader["last_name"].ToString();
                                profile.Gender = reader["gender"] is DBNull ? null : reader["gender"].ToString();
                                profile.BirthDate = reader["birth_date"] is DBNull ? null : reader["birth_date"].ToString();
                                profile.CreatedAt = reader["created_at"] is DBNull ? null : reader["created_at"].ToString();
                                profile.FirebaseId = reader["firebase_id"] is DBNull ? null : reader["firebase_id"].ToString();
                                profile.Role = reader["role"] is DBNull ? null : reader["role"].ToString();
                                profile.ProfilePic = reader["profile_pic"] is DBNull ? null : reader["profile_pic"] as byte[];
                                    // Add other properties as needed
                                await reader.CloseAsync(); // Close the reader

                                await DatabaseManager.CloseConnectionAsync(); 
                                return profile;
                            }
                        }
                        }  catch (TimeoutException)
                            {
                                // Retry after delay
                                retries++;
                                await Task.Delay(TimeSpan.FromSeconds(3)); // Example delay
                            }
                    
                    catch (Exception ex)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                    }
                    
                }
                        await Application.Current.MainPage.DisplayAlert("Error", "Failed to retrieve data after multiple retries.", "OK");

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

            return null;
        }

         public async Task UpdateUserProfileAsync(MyProfileModel profile)
        {
            try
            {
                await DatabaseManager.OpenConnectionAsync();
                using (var command = DatabaseManager.Connection.CreateCommand())
                {
                    command.CommandText = "UPDATE \"Users\" SET \"first_name\" = @first_name, \"last_name\" = @last_name, \"gender\" = @gender WHERE \"firebase_id\" = @firebase_id";

                    command.Parameters.AddWithValue("@first_name", profile.FirstName);
                    command.Parameters.AddWithValue("@last_name", profile.LastName);
                    command.Parameters.AddWithValue("@gender", profile.Gender);
                    command.Parameters.AddWithValue("@firebase_id", profile.FirebaseId);

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                await DatabaseManager.CloseConnectionAsync();
            }
        }
    }
}
