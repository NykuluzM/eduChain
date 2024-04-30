
using CommunityToolkit.Maui.Views;
using eduChain.Models;
using eduChain.Services;
using eduChain.Views.Popups;
using Microsoft.Maui.Controls;

using IAudioManager = Plugin.Maui.Audio.IAudioManager;
namespace eduChain.Views.ContentPages{
	public partial class HomePage : ContentPage
	{
		private HomePageViewModel homePageViewModel;
		private LoadingPopup loadingPopup;
		public HomePage()
		{
			InitializeComponent();
			homePageViewModel = new HomePageViewModel();
            BindingContext = homePageViewModel;
		}
		private async void Load(){
			await LoadProfile();
          
        }
		protected override async void OnAppearing()
		{
			base.OnAppearing();
			
			if(Preferences.Default.Get("isloaded", String.Empty) == "false")
			{
				Load();
				Preferences.Default.Set("isloaded", "true");
			}
			Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;		
		
		}
	
       
		private async Task LoadProfile(){
			try{
				homePageViewModel.IsLoading = true;
	
			    var audioConnection = IPlatformApplication.Current.Services.GetRequiredService<IAudioManager>();
				loadingPopup = new LoadingPopup(audioConnection);
				this.ShowPopup(loadingPopup);
				await homePageViewModel.LoadUsers(Preferences.Default.Get("firebase_uid", string.Empty));
				await Task.Delay(1000);
				//await homePageViewModel.LoadProfileAsync(Preferences.Default.Get("firebase_uid", string.Empty));
				//await Shell.Current.DisplayAlert("Success", "Profile Loaded", null);
				//await Shell.Current.Navigation.PopModalAsync();
			}
			catch{

			}
			finally{
                AppShell appShell = (App.Current as App).MainPage as AppShell;
                Shell.Current.FlyoutIsPresented = true;
				if(DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.Android)
				{
					appShell.TriggerLayout("expanded");
					Shell.Current.FlyoutIsPresented = false;
				} else
				{
                    appShell.TriggerLayout("collapsed");

                }
                await Task.Delay(50);
                loadingPopup.ClosePopup();
				homePageViewModel.IsLoading = false;
			}
		}
		
	}
}