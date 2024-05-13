using eduChain.ViewModels;

namespace eduChain.Views.ContentPages;

public partial class RegisterOrgPage : ContentPage
{
	public RegisterOrgPage()
	{
		InitializeComponent();
		BindingContext = RegisterViewModel.GetInstance();
	}
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
		BindingContext = null;
		RegisterViewModel.ResetInstance();
    }
}