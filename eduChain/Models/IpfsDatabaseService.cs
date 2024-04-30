
namespace eduChain.Models;
public class IpfsDatabaseService
{
        private static IpfsDatabaseService instance;
        private readonly ISupabaseConnection _supabaseConnection;

        public IpfsDatabaseService(ISupabaseConnection supabaseConnection)
        {
            _supabaseConnection = supabaseConnection;
        }
           public static IpfsDatabaseService Instance
        {
            get
            {
                if (instance == null)
                {
                    // Initialize the instance if it's null
                    instance = new IpfsDatabaseService(IPlatformApplication.Current.Services.GetRequiredService<ISupabaseConnection>()); // Add the required service
                }
                return instance;
            }
        }

        public async Task<int> GetPinnedCount(){
            try
            {
                await DatabaseManager.OpenConnectionAsync();
                using(var cmd = DatabaseManager.Connection.CreateCommand())
                {
                    cmd.CommandText = @"SELECT COUNT(*) FROM ""Files""";
                    var result = await cmd.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    return 0; // No pinned items
                }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                return 0;
            }
            finally{
                await DatabaseManager.CloseConnectionAsync();
            }
        }
        public async Task<bool> isPinned(string cid){
            try
            {
                await DatabaseManager.OpenConnectionAsync();
                using(var cmd = DatabaseManager.Connection.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 1 FROM ""Files"" WHERE cid = @cid";
                    cmd.Parameters.AddWithValue("@cid", cid); 
                    var result = await cmd.ExecuteScalarAsync();
                    if(result == null || result == DBNull.Value){
                        return false;
                    }
                    else{
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                return false;
            }
            finally
            {
                await DatabaseManager.CloseConnectionAsync();
            }
        }
     /*   public async Task Task<List<Pin>> GetPinnedFiles()
                   {
            try
        {
                await DatabaseManager.OpenConnectionAsync();
                using (var cmd = DatabaseManager.Connection.CreateCommand())
            {
                    cmd.CommandText = @"SELECT * FROM ""Files""";
                    var reader = await cmd.ExecuteReaderAsync();
                    var pinnedFiles = new List<Pin>();
                    while (await reader.ReadAsync())
                {
                        pinnedFiles.Add(new Pin
                        {
                            Owner = reader.GetString(0),
                            Cid = reader.GetString(1),
                            FileType = reader.GetString(2),
                            FileName = reader.GetString(3)
                        });
                    }
                    return pinnedFiles;
                }
            }
            catch (Exception ex)
        {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                return new List<Pin>();
            }
            finally
        {
                await DatabaseManager.CloseConnectionAsync();
            }
        }*/
        public async Task<List<FileModel>> GetByFileType(string type, string firebase_id)
        {
            var fileList = new List<FileModel>();

            try
            {
                await DatabaseManager.OpenConnectionAsync();
                using (var cmd = DatabaseManager.Connection.CreateCommand())
                {
                        cmd.CommandText = @"SELECT * FROM ""Files"" WHERE filetype = @type && owner = @firebase_id";
                        cmd.Parameters.AddWithValue("@type", type);
                        cmd.Parameters.AddWithValue("@firebase_id", firebase_id);
                        var reader = await cmd.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            fileList.Add(new FileModel
                            {
                                Owner = reader.GetString(0),
                                CID = reader.GetString(1),
                                FileType = reader.GetString(2),
                                FileName = reader.GetString(3),
                            });
                        }
                        return fileList;
                    }   
                }
            catch
            {
                await Shell.Current.DisplayAlert("Error", "An error occurred while fetching the files", "OK");
                return null;
            }
            finally
            {
                await DatabaseManager.CloseConnectionAsync();
            }
        }
        public async Task InsertPinnedFile(string uid,string cid, string fileType, string fileName)
        {
            try
            {
                await DatabaseManager.OpenConnectionAsync();

                using (var cmd = DatabaseManager.Connection.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO ""Files""(owner,cid, filetype, filename) 
                                        VALUES (@uid,@cid, @fileType, @fileName)";

                    cmd.Parameters.AddWithValue("@uid", uid);
                    cmd.Parameters.AddWithValue("@cid", cid);
                    cmd.Parameters.AddWithValue("@fileType", fileType);
                    cmd.Parameters.AddWithValue("@fileName", fileName);

                    await cmd.ExecuteNonQueryAsync(); 
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                // Consider logging the exception for better debugging
            }
            finally
            {
                await DatabaseManager.CloseConnectionAsync();
            }
        }
        public async Task UnpinFile(string cid)
    {
            try
        {
                await DatabaseManager.OpenConnectionAsync();

                using (var cmd = DatabaseManager.Connection.CreateCommand())
            {
                    cmd.CommandText = @"DELETE FROM ""Files"" WHERE cid = @cid";
                    cmd.Parameters.AddWithValue("@cid", cid);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
        {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                // Consider logging the exception for better debugging
            }
            finally
        {
                await DatabaseManager.CloseConnectionAsync();
            }
        }
}