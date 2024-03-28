using eduChain.Model;
using Syncfusion.Maui.DataForm;

namespace eduChain.ViewModel
{
    public class DataFormViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataFormViewModel" /> class.
        /// </summary>
        public DataFormViewModel()
        {
            this.LoginFormModel = new LoginFormModel();

            this.CustomDataFormItemManager = new DataFormItemManagerEditorExt();

        }

        /// <summary>
        /// Gets or sets the login form model.
        /// </summary>
        public LoginFormModel LoginFormModel { get; set; }
        public DataFormItemManager CustomDataFormItemManager { get; set; }

    }

}