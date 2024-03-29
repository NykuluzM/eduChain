using eduChain.ViewModels;
using eduChain.Models;

namespace eduChain.Views.ContentPages{

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
        BindingContext = new LoginViewModel();

	}

        private void TextFieldPasswordShowHideAttachment_LayoutChanged(object sender, EventArgs e)
        {

        }
    }
}