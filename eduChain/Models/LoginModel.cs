using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eduChain.Models
{
   public class LoginModel
    {
        public string Email{ get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}