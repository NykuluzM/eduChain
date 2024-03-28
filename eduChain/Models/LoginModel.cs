using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eduChain.Models
{
   public class LoginModel
    {
        [DisplayName("Username")]
        public string Username{ get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}