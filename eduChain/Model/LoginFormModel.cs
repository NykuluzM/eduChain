using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eduChain.Model
{
    public class LoginFormModel
    {
        [DisplayName("Username")]
        public string Username { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}