using eduChain.Views;

namespace eduChain;
public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		
		MainPage = new AppShell();
		if(Preferences.Get("Email", null) != null){
			AppShell.Current.GoToAsync("//HomePage");
		} else {
			AppShell.Current.GoToAsync("//LoginPage");
		}
	}
}