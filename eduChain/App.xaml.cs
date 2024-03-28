using eduChain.Views;

namespace eduChain;
public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		
		MainPage = new AppShell();


	}
}