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
                    cmd.CommandText = @"SELECT COUNT(*) FROM ""Qr"" WHERE created_by = @created_by";
                    cmd.Parameters.AddWithValue("created_by", firebaseid);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            qrList.Add(new QrModel
                            {
                                Id = reader.GetInt32(0),
                                Expiration = reader.GetDateTime(2)
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
    }
}
