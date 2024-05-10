using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
namespace eduChain.Models
{
    class QrDatabaseService
    {
        private static QrDatabaseService instance;
        private readonly ISupabaseConnection _supabaseConnection;

        public QrDatabaseService(ISupabaseConnection supabaseConnection)
        {
            _supabaseConnection = supabaseConnection;
        }
        public static QrDatabaseService Instance
        {
            get
            {
                if (instance == null)
                {
                    // Initialize the instance if it's null
                    instance = new QrDatabaseService(IPlatformApplication.Current.Services.GetRequiredService<ISupabaseConnection>()); 
                }
                return instance;
            }
        }

        public async Task<ObservableCollection<QrModel>> GetMyQrList(string firebaseid)
        {
            var qrList = new ObservableCollection<QrModel>(); // Initialize your collection

            try
            {
                await DatabaseManager.OpenConnectionAsync();
                using (var cmd = DatabaseManager.Connection.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id, expiration, name FROM ""Qr"" WHERE created_by = @created_by";
                    cmd.Parameters.AddWithValue("created_by", firebaseid);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            qrList.Add(new QrModel
                            {
                                Id = reader.GetInt32(0),
                                Expiration = reader.GetDateTime(1),
                                Name = reader.GetString(2)
                            }) ;
                        }
                    }
                    return qrList;
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                await DatabaseManager.CloseConnectionAsync();
            }
        }
        public async Task<bool> RemoveExpiredQr()
        {
            try
            {
                await DatabaseManager.OpenConnectionAsync();
                using (var cmd = DatabaseManager.Connection.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM ""Qr"" WHERE expiration < @expiration";
                    var  r = cmd.Parameters.AddWithValue("expiration", DateTime.Now.Date);
                    await cmd.ExecuteNonQueryAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                await DatabaseManager.CloseConnectionAsync();
            }
        }
        public async Task<bool> RemoveQr(ObservableCollection<QrModel> qr)
        {
            try
            {
                await DatabaseManager.OpenConnectionAsync();
                foreach (var qrModel in qr)
                {
                    using (var cmd = DatabaseManager.Connection.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM ""Qr"" WHERE id = @id";
                        cmd.Parameters.AddWithValue("id", qrModel.Id);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                await DatabaseManager.CloseConnectionAsync();
            }
        }
    }
}
