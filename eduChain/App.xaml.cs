using CommunityToolkit.Maui.Core.Views;
using eduChain.Views;
using eduChain.Views.ContentPages;
namespace eduChain;
using Microsoft.Maui.Storage;
public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		MainPage = new AppShell();
	}
}