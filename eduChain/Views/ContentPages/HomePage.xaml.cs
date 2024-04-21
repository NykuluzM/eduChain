
using CommunityToolkit.Maui.Views;
using eduChain.Services;
using eduChain.Views.Popups;
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
		protected override void OnAppearing()
		{
			base.OnAppearing();
			if(Preferences.Default.Get("isloaded", String.Empty) == "false")
			{
				Load();
				Preferences.Default.Set("isloaded", "true");
			}
		}
		private async Task LoadProfile(){
			try{
				homePageViewModel.IsLoading = true;
	
			    var audioConnection = IPlatformApplication.Current.Services.GetRequiredService<IAudioManager>();
				loadingPopup = new LoadingPopup(audioConnection);
				this.ShowPopup(loadingPopup);
				await Task.Delay(5000);
				await homePageViewModel.LoadUsers(Preferences.Default.Get("firebase_uid", string.Empty));
				await homePageViewModel.LoadProfileAsync(Preferences.Default.Get("firebase_uid", string.Empty));
			}
			catch{

			}
			finally{
				loadingPopup.ClosePopup();
				homePageViewModel.IsLoading = false;

			}
		}
		
	}
}