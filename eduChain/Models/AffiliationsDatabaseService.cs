using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eduChain.Models
{
    class AffiliationsDatabaseService
    {
        private static AffiliationsDatabaseService instance;
        private readonly ISupabaseConnection _supabaseConnection;

        public AffiliationsDatabaseService(ISupabaseConnection supabaseConnection)
        {
            _supabaseConnection = supabaseConnection;
        }
        public static AffiliationsDatabaseService Instance
        {
            get
            {
                if (instance == null)
                {
                    // Initialize the instance if it's null
                    instance = new AffiliationsDatabaseService(IPlatformApplication.Current.Services.GetRequiredService<ISupabaseConnection>()); // Add the required service
                }
                return instance;
            }
        }

        public async Task<int> GetPinnedCount()
        {
            try
            {
                await DatabaseManager.OpenConnectionAsync();
                using (var cmd = DatabaseManager.Connection.CreateCommand())
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
            finally
            {
                await DatabaseManager.CloseConnectionAsync();
            }
        }
        public async Task<bool> Exists(string cid, string cat)
        {
            string decision = "";
            if(cat == "organization")
            {
                decision = "Organizations";
            }
            else if(cat == "student")
            {
                decision = "Students";
            }
            else
            {
                return false;
            }
            try
            {
                await DatabaseManager.OpenConnectionAsync();
                using (var cmd = DatabaseManager.Connection.CreateCommand())
                {
                    cmd.CommandText = $@"SELECT COUNT(*) FROM ""{decision}"" WHERE ""user_firebase_id"" = @cid";
                    cmd.Parameters.AddWithValue("@cid", cid);
                    cmd.Parameters.AddWithValue("@decision", decision);
                   
                    var result = await cmd.ExecuteScalarAsync();
                    int count = Convert.ToInt32(result);

                    if (result != null && result != DBNull.Value && count == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false; // No pinned items
                    }
                }
            } catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                return false;
            }
            finally
            {
                await DatabaseManager.CloseConnectionAsync();
            }
        }
        public async Task<ObservableCollection<AffiliationsModel>> GetAffiliationRequestsStudents(string mycid)
        {
            try
            {
                await DatabaseManager.OpenConnectionAsync();
                using (var cmd = DatabaseManager.Connection.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT A.*, CONCAT(S.first_name, ' ', S.last_name) AS StudentName
                FROM ""Affiliations"" A
                INNER JOIN ""Students"" S ON A.affiliate = S.user_firebase_id 
                WHERE ""affiliated_to"" = @mycid AND ""approved"" = @approved";
                    cmd.Parameters.AddWithValue("@mycid", mycid);
                    cmd.Parameters.AddWithValue("@approved", false);
                    var result = await cmd.ExecuteReaderAsync();

                    var affiliations = new List<AffiliationsModel>();

                    while (await result.ReadAsync())
                    {
                        var affiliation = new AffiliationsModel
                        {
                            Id = result.GetString(3),
                            Name = result.GetString(result.GetOrdinal("StudentName")),
                            DateEstablished = result.GetDateTime(result.GetOrdinal("created_at")).ToString("yyyy-MM-dd")
                        };
                        affiliations.Add(affiliation);
                    }

                    var observableCollection = new ObservableCollection<AffiliationsModel>(affiliations);
                    return observableCollection;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                return null;
            }
            finally
            {
                await DatabaseManager.CloseConnectionAsync();
            }
        }
        public async Task DeleteRequest(string affiliate, string affiliated_to)
        {
            try
            {
                await DatabaseManager.OpenConnectionAsync();
                using (var cmd = DatabaseManager.Connection.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM ""Affiliations"" WHERE ""affiliate"" = @affiliate AND ""affiliated_to"" = @affiliated_to";
                    cmd.Parameters.AddWithValue("@affiliate", affiliate);
                    cmd.Parameters.AddWithValue("@affiliated_to", affiliated_to);

                    await cmd.ExecuteNonQueryAsync();
                }
                await Shell.Current.DisplayAlert("Success", "Request deleted", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                await DatabaseManager.CloseConnectionAsync();
            }
        }
        public async Task<ObservableCollection<AffiliationsModel>> GetAffiliationRequestsOrganizations(string mycid)
        {
            try
            {
                await DatabaseManager.OpenConnectionAsync();
                using (var cmd = DatabaseManager.Connection.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT A.*, O.name AS OrganizationName
                FROM ""Affiliations"" A
                INNER JOIN ""Organizations"" O ON A.affiliate = O.user_firebase_id 
                WHERE ""affiliated_to"" = @mycid AND ""approved"" = @approved";
                    cmd.Parameters.AddWithValue("@mycid", mycid);
                    cmd.Parameters.AddWithValue("@approved", false);

                    var result = await cmd.ExecuteReaderAsync();
                    var affiliations = new List<AffiliationsModel>();

                    while (await result.ReadAsync())
                    {
                        var affiliation = new AffiliationsModel
                        {
                            Id = result.GetString(3),
                            Name = result.GetString(result.GetOrdinal("OrganizationName")),
                            DateEstablished = result.GetDateTime(result.GetOrdinal("created_at")).ToString("yyyy-MM-dd")
                        };
                        affiliations.Add(affiliation);
                    }

                    var observableCollection = new ObservableCollection<AffiliationsModel>(affiliations);
                    return observableCollection;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                return null;
            }
            finally
            {
                await DatabaseManager.CloseConnectionAsync();
            }
        }

        public async Task SendAffiliationRequest(string affiliate, string affiliated_to)
        {
            try
            {
                await DatabaseManager.OpenConnectionAsync();
                using (var cmd = DatabaseManager.Connection.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO ""Affiliations"" (affiliate, affiliated_to, approved) VALUES (@affiliate, @affiliated_to, @approved)";
                    cmd.Parameters.AddWithValue("@affiliate", affiliate);
                    cmd.Parameters.AddWithValue("@affiliated_to", affiliated_to);
                    cmd.Parameters.AddWithValue("@approved", false);

                    await cmd.ExecuteNonQueryAsync();
                }
                await Shell.Current.DisplayAlert("Success", "Affiliation request sent", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error Affiliation Request", ex.Message, "OK");
                await Shell.Current.DisplayAlert("Error Affiliation Request", "Do not input already affiliated users or self affiliate", "OK");
            }
            finally
            {
                await DatabaseManager.CloseConnectionAsync();
            }
        }

        public async Task<ObservableCollection<AffiliationsModel>> GetAffiliatedOrganizationTo(string firebase_id)
        {
            try
            {
                await DatabaseManager.OpenConnectionAsync();
                using (var cmd = DatabaseManager.Connection.CreateCommand())
                {
                    // Modify the SQL query to join the Affiliations table with the Organizations table
                    cmd.CommandText = @"
                SELECT A.*, O.name AS OrganizationName 
                FROM ""Affiliations"" A
                INNER JOIN ""Organizations"" O ON A.affiliated_to = O.user_firebase_id
                WHERE A.affiliate = @firebase_id AND A.approved = @approved";
                    cmd.Parameters.AddWithValue("@firebase_id", firebase_id);
                    cmd.Parameters.AddWithValue("@approved", true);
                    var result = await cmd.ExecuteReaderAsync();
                    var affiliations = new List<AffiliationsModel>();
                    while (await result.ReadAsync())
                    {
                        var affiliation = new AffiliationsModel
                        {
                            Id = result.GetString(3), // Assuming Id is the index of Affiliations.Id
                            Name = result.GetString(result.GetOrdinal("OrganizationName")),
                            DateEstablished = result.GetDateTime(result.GetOrdinal("created_at")).ToString("yyyy-MM-dd")
                        };
                        affiliations.Add(affiliation);
                    }
                    var observableCollection = new ObservableCollection<AffiliationsModel>(affiliations);

                    return observableCollection;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                return null;
            }
            finally
            {
                await DatabaseManager.CloseConnectionAsync();
            }
         
        }
        public async Task<ObservableCollection<AffiliationsModel>> GetAffiliatedStudentsTo(string firebase_id)
        {
            try
            {
                await DatabaseManager.OpenConnectionAsync();
                using (var cmd = DatabaseManager.Connection.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT A.*, CONCAT(S.first_name, ' ', S.last_name) AS StudentName 
                FROM ""Affiliations"" A
                INNER JOIN ""Students"" S ON A.affiliated_to = S.user_firebase_id 
                WHERE A.affiliate = @firebase_id AND A.approved = @approved";
                    cmd.Parameters.AddWithValue("@firebase_id", firebase_id);
                    cmd.Parameters.AddWithValue("@approved", true);
                    var result = await cmd.ExecuteReaderAsync();
                    var affiliations = new List<AffiliationsModel>();

                    while (await result.ReadAsync())
                    {
                        var affiliation = new AffiliationsModel
                        {
                            Id = result.GetString(3), // Assuming Id is still the index of Affiliations.Id
                            Name = result.GetString(result.GetOrdinal("StudentName")),
                            DateEstablished = result.GetDateTime(result.GetOrdinal("created_at")).ToString("yyyy-MM-dd")
                        };
                        affiliations.Add(affiliation);
                    }
                    var observableCollection = new ObservableCollection<AffiliationsModel>(affiliations);
                    return observableCollection;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
                return null;
            }
            finally
            {
                await DatabaseManager.CloseConnectionAsync();
            }
        }
    }
}
