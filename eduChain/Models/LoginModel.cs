using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eduChain.Models
{
   public class LoginModel
    {
        public string Username{ get; set; }
        public string Password { get; set; }
    }
}