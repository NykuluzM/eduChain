using eduChain.Models;
using eduChain.Models.MyProfileModels;
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
        public async Task<OrganizationProfileModel> UserProfileAsync(string uid, OrganizationProfileModel profile)
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

                    string sqlQuery = @"
                                                SELECT *
                                                FROM ""Organizations""
                                                WHERE ""user_firebase_id"" = @firebase_id";
                    command.CommandText = sqlQuery;
                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {

                                profile.Email = Preferences.Default.Get("email", String.Empty);
                                profile.Name = reader["name"] is DBNull ? null : reader["name"].ToString();
                                profile.Type = reader["type"] is DBNull ? null : reader["type"].ToString();
                                profile.FirebaseId = reader["user_firebase_id"] is DBNull ? null : reader["user_firebase_id"].ToString();
                                // Add other properties as needed
                                await reader.CloseAsync(); // Close the reader

                                await DatabaseManager.CloseConnectionAsync();
                                OrganizationProfileModel.Instance = profile;
                                return profile;
                            }
                        }
                    }
                    catch (TimeoutException)
                    {
                        // Retry after delay
                        await Task.Delay(TimeSpan.FromSeconds(3)); // Example delay
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception appropriately
                        await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
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
        public async Task<StudentProfileModel> UserProfileAsync(string uid, StudentProfileModel profile)
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

                    string sqlQuery = @"
                                                SELECT *
                                                FROM ""Students""
                                                WHERE ""user_firebase_id"" = @firebase_id";
                    command.CommandText = sqlQuery;
                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {

                                profile.Email = Preferences.Default.Get("email", String.Empty);
                                profile.FirstName = reader["first_name"] is DBNull ? null : reader["first_name"].ToString();
                                profile.LastName = reader["last_name"] is DBNull ? null : reader["last_name"].ToString();
                                profile.Gender = reader["gender"] is DBNull ? null : reader["gender"].ToString();
                                profile.BirthDate = reader["birth_date"] is DBNull ? null : reader["birth_date"].ToString();
                                profile.FirebaseId = reader["user_firebase_id"] is DBNull ? null : reader["user_firebase_id"].ToString();
                                // Add other properties as needed
                                await reader.CloseAsync(); // Close the reader

                                await DatabaseManager.CloseConnectionAsync();
                                StudentProfileModel.Instance = profile;
                                return profile;
                            }
                        }
                    }
                    catch (TimeoutException)
                    {
                        // Retry after delay
                        await Task.Delay(TimeSpan.FromSeconds(3)); // Example delay
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception appropriately
                        await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
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

        public async Task UpdateStudentUserProfileAsync(StudentProfileModel profile, string displayname)
        {
            try
            {
                await DatabaseManager.OpenConnectionAsync();
                using (var command = DatabaseManager.Connection.CreateCommand())
                {
                  
                            command.CommandText = @"
                                            UPDATE ""Students"" 
                                            SET ""first_name"" = @first_name, ""last_name"" = @last_name
                                            WHERE ""user_firebase_id"" = @firebase_id";
                            command.Parameters.AddWithValue("@first_name", profile.FirstName);
                            command.Parameters.AddWithValue("@last_name", profile.LastName);
                            command.Parameters.AddWithValue("@firebase_id", profile.FirebaseId);

                            await command.ExecuteNonQueryAsync();
                    command.CommandText = @"
                                            UPDATE ""Users"" 
                                            SET ""display_name"" = @display_name
                                            WHERE ""firebase_id"" = @firebase_id";
                    command.Parameters.AddWithValue("@display_name", displayname);
                    await command.ExecuteNonQueryAsync();
                    Shell.Current.DisplayAlert("Success", "Profile updated successfully", "OK");
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

        public async Task UpdateOrganizationUserProfileAsync(OrganizationProfileModel profile, string displayname)
        {
            try
            {
                await DatabaseManager.OpenConnectionAsync();
                using (var command = DatabaseManager.Connection.CreateCommand())
                {

                    command.CommandText = @"
                                            UPDATE ""Organizations"" 
                                            SET ""name"" = @_name
                                            WHERE ""user_firebase_id"" = @firebase_id";
                    command.Parameters.AddWithValue("@_name", profile.Name);
                    command.Parameters.AddWithValue("@firebase_id", profile.FirebaseId);

                    await command.ExecuteNonQueryAsync();
                    command.CommandText = @"
                                            UPDATE ""Users"" 
                                            SET ""display_name"" = @display_name
                                            WHERE ""firebase_id"" = @firebase_id";
                    command.Parameters.AddWithValue("@display_name", displayname);
                    await command.ExecuteNonQueryAsync();
                    Shell.Current.DisplayAlert("Success", "Profile updated successfully", "OK");
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

        public async Task<UsersProfileModel> LoadUserAsync(string uid, UsersProfileModel profile)
        {
            try
            {
                await DatabaseManager.OpenConnectionAsync();

                using (var command = DatabaseManager.Connection.CreateCommand())
                {
                    command.Parameters.AddWithValue("@firebase_id", uid);
                    command.CommandText = "SELECT * FROM \"Users\" WHERE \"firebase_id\" = @firebase_id";

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            profile.FirebaseId = reader.GetString(reader.GetOrdinal("firebase_id"));
                            int profilePicOrdinal = reader.GetOrdinal("profile_pic");
                            if (!reader.IsDBNull(profilePicOrdinal))
                            {
                                profile.ProfilePic = reader.GetFieldValue<byte[]>(profilePicOrdinal);
                            }
                            else
                            {
                                profile.ProfilePic = null;
                            }
                            profile.Role = reader.GetString(reader.GetOrdinal("role"));
                            profile.FirebaseId = reader.GetString(reader.GetOrdinal("firebase_id"));
                            profile.CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"));
                            profile.DisplayName = reader.GetString(reader.GetOrdinal("display_name"));
                            await reader.CloseAsync();
                            if (profile.Role == "Student")
                            {
                                // Query additional information for student profile
                                command.CommandText = "SELECT * FROM \"Students\" WHERE \"user_firebase_id\" = @firebase_id";
                                using (var secondReader = await command.ExecuteReaderAsync())
                                {
                                    if (await secondReader.ReadAsync())
                                    {
                                        string firstName = secondReader.GetString(secondReader.GetOrdinal("first_name"));
                                        string lastName = secondReader.GetString(secondReader.GetOrdinal("last_name"));
                                    }
                                    await secondReader.CloseAsync();
                                }
                            }
                            else if (profile.Role == "Organization")
                            {
                                // Query additional information for organization profile
                                command.CommandText = "SELECT * FROM \"Organizations\" WHERE \"user_firebase_id\" = @firebase_id";
                                using (var secondReader = await command.ExecuteReaderAsync())
                                {
                                    if (await secondReader.ReadAsync())
                                    {
                                        string orgName = secondReader.GetString(secondReader.GetOrdinal("name"));
                                        await secondReader.CloseAsync();
                                    }
                                }
                            }

                        }
                    }
                    Preferences.Default.Set("Role", profile.Role);
                    await DatabaseManager.CloseConnectionAsync();
                    return profile;
                }
            }
            catch (Exception ex)
            {
                // Handle database errors gracefully (log, display error message)
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                return null;
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