using Syncfusion.Maui.DataForm;
using eduChain.Models;


namespace eduChain.ViewModels
{
    public class LoginViewModel
    {
        /// <summary>
    /// Initializes a new instance of the <see cref="DataFormViewModel" /> class.
    /// </summary>
    public LoginViewModel()
    {
       this.LoginModel = new LoginModel();
       
       //this.CustomDataFormItemManager = new DataFormItemManagerEditorExt();

    }
    
    /// <summary>
    /// Gets or sets the login form model.
    /// </summary>
    public LoginModel LoginModel { get; set; }
    public DataFormItemManager CustomDataFormItemManager { get; set; }

    }
    
}