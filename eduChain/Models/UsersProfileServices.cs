using eduChain.Models;

namespace eduChain.Models;
using Npgsql;
public class UsersProfileServices
{        private static UsersProfileServices instance;
      private readonly ISupabaseConnection _supabaseConnection;
      
        public UsersProfileServices(ISupabaseConnection supabaseConnection)
        {
            _supabaseConnection = supabaseConnection;
        }
          public static UsersProfileServices Instance
        {
            get
            {
                if (instance == null)
                {
                    // Initialize the instance if it's null
                    instance = new UsersProfileServices(IPlatformApplication.Current.Services.GetRequiredService<ISupabaseConnection>()); // Add the required service
                }
                return instance;
            }
        }

        public async Task<List<UsersModel>> LoadProfileCardsAsync()
        {
            List<UsersModel> users = new List<UsersModel>();

    try
    {
        await DatabaseManager.OpenConnectionAsync();

        string query = "SELECT * FROM Users";
        NpgsqlCommand cmd = new NpgsqlCommand(query, DatabaseManager.Connection);
        NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            // Map database columns to UserModel properties
            UsersModel user = new UsersModel
            {
                //Id = reader.GetInt32(reader.GetOrdinal("Id")),
                //Name = reader.GetString(reader.GetOrdinal("Name")),
                // Map other properties accordingly
            };

            users.Add(user);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred while loading profile cards: " + ex.Message);
        // Log the error or handle it appropriately
    }
    finally
    {
        await DatabaseManager.CloseConnectionAsync();
    }

    return users;
}
 
}
