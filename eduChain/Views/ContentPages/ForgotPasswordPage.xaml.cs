using eduChain.Models;
using eduChain.ViewModels;

namespace eduChain.Views.ContentPages{
public partial class ForgotPasswordPage : ContentPage
{
	private readonly FirebaseService _firebaseService;
			private ForgotPasswordViewModel viewModel;
	public ForgotPasswordPage()
	{
		InitializeComponent();
		viewModel =	ForgotPasswordViewModel.GetInstance();
		BindingContext = viewModel;
	}

	private void ResetPasswordButton_Clicked(object sender, EventArgs e)
	{
	}
}
}