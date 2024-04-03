namespace eduChain;
using System.Data;
public interface ISupabaseConnection
{
    IDbConnection GetConnection();
}
