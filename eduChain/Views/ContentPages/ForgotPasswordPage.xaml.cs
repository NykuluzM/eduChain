using eduChain.Models;
using eduChain.ViewModels;

namespace eduChain.Views.ContentPages{
public partial class ForgotPasswordPage : ContentPage
{
	public ForgotPasswordPage()
	{
		InitializeComponent();
		BindingContext = ForgotPasswordViewModel.GetInstance();
	}
}
}