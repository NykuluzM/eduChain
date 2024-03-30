namespace eduChain.Models{

public class RegisterModel
{
    public string firstName{ get; set; } = string.Empty;
    public string lastName{ get; set; } = string.Empty;
    public string Birthday{ get; set; } = string.Empty;
    public string Email{ get; set; }
    public string Password { get; set; } = string.Empty;
}

}