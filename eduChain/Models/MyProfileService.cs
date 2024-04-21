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
            var role = await GetUserRoleAsync(uid);
            try
            {
                await DatabaseManager.OpenConnectionAsync();
                using (var command = DatabaseManager.Connection.CreateCommand())
                {   
                   
                    var parameters = command.Parameters;

                    if (parameters is NpgsqlParameterCollection pgParameters)
                    {
                        pgParameters.AddWithValue("@firebase_id", uid);
                    }
                     switch (role){
                        case "Student":
                            string sqlQuery = @"
                                                SELECT u.*, s.*
                                                FROM ""Users"" u
                                                INNER JOIN ""Students"" s ON u.""firebase_id"" = s.""user_firebase_id""
                                                WHERE u.""firebase_id"" = @firebase_id";
                            command.CommandText = sqlQuery;
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
                                await Task.Delay(TimeSpan.FromSeconds(3)); // Example delay
                            }
                            break;
                        case "Organization":
                            string organizationSqlQuery = @"
                                                            SELECT u.*, o.*
                                                            FROM ""Users"" u
                                                            INNER JOIN ""Organizations"" o ON u.""firebase_id"" = o.""user_firebase_id""
                                                            WHERE u.""firebase_id"" = @firebase_id"; 
                            command.CommandText = organizationSqlQuery;
                            try
                            {
                                using (var reader = await command.ExecuteReaderAsync())
                                {
                                    if (await reader.ReadAsync())
                                    {
                                        profile.Email = Preferences.Default.Get("email", String.Empty);
                                        // Update other profile properties based on the organization data
                                        profile.OrgName = reader["name"] is DBNull ? null : reader["name"].ToString();
                                        profile.FirebaseId = reader["firebase_id"] is DBNull ? null : reader["firebase_id"].ToString();
                                        profile.Role = reader["role"] is DBNull ? null : reader["role"].ToString();
                                        profile.ProfilePic = reader["profile_pic"] is DBNull ? null : reader["profile_pic"] as byte[];
                                        // Add other properties as needed
                                        await reader.CloseAsync(); // Close the reader
                                        await DatabaseManager.CloseConnectionAsync();
                                        return profile;
                                    }
                                }
                            }
                            catch (TimeoutException)
                            {
                                // Retry after delay
                                await Task.Delay(TimeSpan.FromSeconds(3)); // Example delay
                            }
                            break;
                        case "Guardian":
                            command.CommandText = "SELECT * FROM \"Guardians\" WHERE \"firebase_id\" = @firebase_id";
                            break;      
                        default:
                            command.CommandText = "SELECT * FROM \"Users\" WHERE \"firebase_id\" = @firebase_id";
                            break;
                    }
                     
                      
                    
                }
                        await Application.Current.MainPage.DisplayAlert("Error", "Failed to retrieve data after multiple retries.", "OK");

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
                var role = await GetUserRoleAsync(Preferences.Default.Get("firebase_uid", String.Empty));
                await DatabaseManager.OpenConnectionAsync();
                using (var command = DatabaseManager.Connection.CreateCommand())
                {
                    switch(role){
                        case "Student":
                            command.CommandText = @"
                                            UPDATE ""Students"" 
                                            SET ""first_name"" = @first_name, ""last_name"" = @last_name
                                            WHERE ""user_firebase_id"" = @firebase_id";
                            command.Parameters.AddWithValue("@first_name", profile.FirstName);
                            command.Parameters.AddWithValue("@last_name", profile.LastName);
                            command.Parameters.AddWithValue("@firebase_id", profile.FirebaseId);

                            await command.ExecuteNonQueryAsync();
                        break;
                        case "Organization":
                        break;

                        case "Guardian":
                        break;
                    }
              
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
        public async Task<string> GetUserRoleAsync(string uid)
        {
            try
            {
                await DatabaseManager.OpenConnectionAsync();
                using (var command = DatabaseManager.Connection.CreateCommand())
                {
                    var parameters = command.Parameters;

                    if (parameters is NpgsqlParameterCollection pgParameters)
                    {
                        pgParameters.AddWithValue("@firebase_id", uid);
                    }

                    command.CommandText = "SELECT \"role\" FROM \"Users\" WHERE \"firebase_id\" = @firebase_id";

                    var result = await command.ExecuteScalarAsync();

                    if (result != null && result != DBNull.Value)
                    {
                        return result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                await DatabaseManager.CloseConnectionAsync();
            }
            // Return null if no role is found for the given firebase_id
            return null;
        }
    }
}
