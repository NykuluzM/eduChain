
using System.Data;
using Npgsql;

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
        public async Task<List<FileModel>> GetAllFilesAsync(string firebase_id){
            var fileList = new List<FileModel>();
            try
            {
                await DatabaseManager.OpenConnectionAsync();
                using (var cmd = DatabaseManager.Connection.CreateCommand())
                {
                    cmd.CommandText = @"SELECT created_by,cid,filetype,filename FROM ""Files"" WHERE created_by = @firebase_id";
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
            catch(Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.ToString(), "OK");
                return null;
            }
            finally
            {
                await DatabaseManager.CloseConnectionAsync();
            }
        }
    public async Task<List<FileModel>> GetAllSharedFiles(string firebase_id)
    {
        var fileList = new List<FileModel>();
        try
        {
            await DatabaseManager.OpenConnectionAsync();
            using (var cmd = DatabaseManager.Connection.CreateCommand())
            {
                cmd.CommandText = @"
                SELECT F.owner,F.created_by, F.cid, F.filename, F.filetype, F.filename 
                FROM ""Files"" F
                INNER JOIN ""Affiliations"" A ON F.created_by = A.affiliated_to
                WHERE F.owner = @firebase_id AND A.affiliate = @firebase_id AND A.approved = true";
                cmd.Parameters.AddWithValue("@firebase_id", firebase_id);

                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    fileList.Add(new FileModel
                    {
                        Owner = reader.GetString(1),
                        CID = reader.GetString(2),
                        FileType = reader.GetString(4),
                        FileName = reader.GetString(3),
                    });
                }
                return fileList;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.ToString(), "OK");
            return null;
        }
        finally
        {
            await DatabaseManager.CloseConnectionAsync();
        }
    }
    public async Task<List<(string CID, string Filename)>> RetrieveFilenames(List<string> cidList)
    {
        List<(string CID, string Filename)> filenameList = new List<(string, string)>();
        try
        {
            await DatabaseManager.OpenConnectionAsync();
            using (var cmd = DatabaseManager.Connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT cid, filename FROM ""Files"" WHERE cid = ANY(@cidList)";
                cmd.Parameters.AddWithValue("@cidList", cidList.ToArray());
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    filenameList.Add((reader.GetString(0), reader.GetString(1)));
                }
                return filenameList;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.ToString(), "OK");
            return null;
        }
        finally
        {
            await DatabaseManager.CloseConnectionAsync();
        }
    }

    public async Task<List<FileModel>> RefreshFilesAsync(string firebase_id, DateTime lastRefreshed)
    {
            var fileList = new List<FileModel>();
            try
        {
                await DatabaseManager.OpenConnectionAsync();
                using (var cmd = DatabaseManager.Connection.CreateCommand())
            {
                    cmd.CommandText = @"SELECT owner,cid,filetype,filename FROM ""Files"" WHERE owner = @firebase_id AND pinned_at >= @pinned_at";
                    cmd.Parameters.AddWithValue("@firebase_id", firebase_id);
                cmd.Parameters.AddWithValue("@pinned_at", lastRefreshed);
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
            catch (Exception ex)
        {
                await Shell.Current.DisplayAlert("Error", ex.ToString(), "OK");
                return null;
            }
            finally
        {
                await DatabaseManager.CloseConnectionAsync();
            }
        }   

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
        public async Task<bool> InsertPinnedFile(string uid,string created_by,string cid, string fileType, string fileName, string owner)
        {
            var isValid = false;
            try
            {
                await DatabaseManager.OpenConnectionAsync();

                using (var cmd = DatabaseManager.Connection.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO ""Files""(owner,created_by,cid, filetype, filename) 
                                        VALUES (@owner,@created_by,@cid, @fileType, @fileName)";

                    cmd.Parameters.AddWithValue("@owner",owner);
                    cmd.Parameters.AddWithValue("@cid", cid);
                    cmd.Parameters.AddWithValue("@fileType", fileType);
                    cmd.Parameters.AddWithValue("@fileName", fileName);
                    cmd.Parameters.AddWithValue("@created_by", uid);
  
                    await cmd.ExecuteNonQueryAsync(); 
                }
                isValid = true;
                return isValid;
            }
            catch (Exception ex)
            {
                isValid = false;
                return isValid;
            }
            finally
            {
                await DatabaseManager.CloseConnectionAsync();
            }
        }
        private readonly SemaphoreSlim qrRegistrationSemaphore = new SemaphoreSlim(1);

        public async Task<int> QrCodeRegister(DateOnly expiration){
            try
            {
                await qrRegistrationSemaphore.WaitAsync(); // Wait for access
                await DatabaseManager.OpenConnectionAsync();
                using (var cmd = DatabaseManager.Connection.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO ""Qr"" (expiration) VALUES (@expiration) RETURNING id";
                    cmd.Parameters.AddWithValue("@expiration", expiration);
                    var rowsAffected = await cmd.ExecuteNonQueryAsync();
                    // Check if any rows were inserted
                    if (rowsAffected > 0)
                    {
                        var insertedId = Convert.ToInt32(rowsAffected); // Get the first ID from OUTPUT
                        return insertedId;
                    }
                    else
                    {
                        throw new Exception("Failed to Generate QR Code, Try Again Later");
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                await Shell.Current.DisplayAlert("Error", "Failed to Implement QR, Try Again Later", "OK");
                return 0;
            }
            finally
            {
                await DatabaseManager.CloseConnectionAsync();
                qrRegistrationSemaphore.Release(); // Release access

            }
        }
        public async Task<bool> QrCodeViable(int id){
            try{
                await DatabaseManager.OpenConnectionAsync();
                using (var cmd = DatabaseManager.Connection.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 1 FROM ""Qr"" WHERE id = @id AND expiration >= @check";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@check", DateTime.Now.Date);
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
                await Shell.Current.DisplayAlert("Error", "Failed to Decode QR, Try Again Later", "OK");
                return false;
            }
            finally
            {
                await DatabaseManager.CloseConnectionAsync();
            
            }
        }
        public async Task<bool> RevokeQrCode(int id){
            try{
                await DatabaseManager.OpenConnectionAsync();
                using (var cmd = DatabaseManager.Connection.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM ""Qr"" WHERE id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to Revoke QR, Try Again Later", "OK");
                return false;
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