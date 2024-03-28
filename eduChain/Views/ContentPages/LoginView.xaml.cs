using Syncfusion.Maui.DataForm;	
using eduChain.ViewModels;

namespace eduChain.Views.ContentPages{

public partial class LoginView : ContentPage
{
	public LoginView()
	{
		var viewModel = new LoginViewModel();
            BindingContext = viewModel;

		InitializeComponent();
	}
}
}