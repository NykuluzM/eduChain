using CommunityToolkit.Maui.Core.Views;
using eduChain.Views;
using eduChain.Views.ContentPages;
using Microsoft.Maui.Storage;
using Microsoft.Maui.Controls;
using eduChain.Models;
using Firebase.Auth;
namespace eduChain{
public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		MainPage = new AppShell();
	}
   
}
}