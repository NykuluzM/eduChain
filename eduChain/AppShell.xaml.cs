namespace eduChain;
using eduChain.Views.ContentPages;
using Microsoft.Maui.Storage;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute(Routes.RegisterPage, typeof(RegisterPage));
		Routing.RegisterRoute("forgotPasswordPage", typeof(ForgotPasswordPage));
	}
}
