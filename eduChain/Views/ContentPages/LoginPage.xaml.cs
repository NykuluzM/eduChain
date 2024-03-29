using Microsoft.Maui.Controls;
using eduChain.ViewModels;
using eduChain.Models;

namespace eduChain.Views.ContentPages{

public partial class LoginPage : ContentPage
{

	public LoginPage()
	{
		InitializeComponent();

	}

        private void TextFieldPasswordShowHideAttachment_LayoutChanged(object sender, EventArgs e)
        {

        }
        private void LoginButton_Clicked(object sender, EventArgs e)
        {
            // Handle the Login button click event
                ((LoginViewModel)BindingContext).Login();

        }
        private void RegisterButton_Clicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//registerPage");
        }
    }
}