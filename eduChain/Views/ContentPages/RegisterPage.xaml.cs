using eduChain.ViewModels;

namespace eduChain.Views.ContentPages
{
	public partial class RegisterPage : ContentPage
	{
		public RegisterPage()
		{
			InitializeComponent();
			var registerViewModel = new RegisterViewModel();
			BindingContext = registerViewModel;
		}

    }
}
