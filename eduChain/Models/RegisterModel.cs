
using Postgrest.Attributes;
using Postgrest.Models;

namespace eduChain.Models;

[Table("users")]
public class RegisterModel : BaseModel
{
    // public string firstName{ get; set; } = string.Empty;
    // public string lastName{ get; set; } = string.Empty;
    // public string Birthday{ get; set; } = string.Empty;
    // public string Email{ get; set; }
    // public string Password { get; set; } = string.Empty;
   [PrimaryKey("id")]
    public string Email { get; set; }

    [Column("created_at")]
    public string CreatedAt { get; set; }
    
}

