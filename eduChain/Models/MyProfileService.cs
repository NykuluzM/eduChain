using eduChain.Models;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace eduChain.Services
{
    public class MyProfileService
    {
        private readonly ISupabaseConnection _supabaseConnection;

        public MyProfileService(ISupabaseConnection supabaseConnection)
        {
            _supabaseConnection = supabaseConnection;
        }

        public async Task<MyProfileModel> GetUserProfileAsync(string uid)
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

                    command.CommandText = "SELECT * FROM \"Users\" WHERE \"firebase_id\" = @firebase_id";
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            MyProfileModel profile = new MyProfileModel
                            {
                                Email = Preferences.Default.Get("email", String.Empty),
                                FirstName = reader["first_name"] is DBNull ? null : reader["first_name"].ToString(),
                                LastName = reader["last_name"] is DBNull ? null : reader["last_name"].ToString(),
                                Gender = reader["gender"] is DBNull ? null : reader["gender"].ToString(),
                                CreatedAt = reader["created_at"] is DBNull ? null : reader["created_at"].ToString(),
                                FirebaseId = reader["firebase_id"] is DBNull ? null : reader["firebase_id"].ToString(),
                                ProfilePic = reader["profile_pic"] is DBNull ? null : reader["profile_pic"] as byte[],
                                // Add other properties as needed
                            };
                            await DatabaseManager.CloseConnectionAsync(); 
                            return profile;
                        }
                    }
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
    }
}
