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
        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);
			const int width = 1000;
			const int height = 450;

			window.X = 100;
			window.Y = 200;

			window.MinimumWidth = width;
			window.MinimumHeight = height;
			Dispatcher.Dispatch(() =>
            {
                window.MinimumWidth = width;
                window.MinimumHeight = height;
                window.MaximumWidth = double.PositiveInfinity;
                window.MaximumHeight = double.PositiveInfinity;
            });			
			return window;
        }
    }
}