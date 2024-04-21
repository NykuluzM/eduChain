namespace eduChain.Views.ContentPages;

public partial class RegisterGuardianPage : ContentPage
{
	public RegisterGuardianPage()
	{
		InitializeComponent();
	}
	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		BindingContext = null;	
	}
}