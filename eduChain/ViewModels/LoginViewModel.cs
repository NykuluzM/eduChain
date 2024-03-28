using eduChain.Models;


namespace eduChain.ViewModels
{
    public class LoginViewModel
    {
        /// <summary>
    /// </summary>
    public LoginViewModel()
    {
       this.LoginModel = new LoginModel();

       //this.CustomDataFormItemManager = new DataFormItemManagerEditorExt();

    }
    public LoginModel LoginModel { get; set; }

    }
}