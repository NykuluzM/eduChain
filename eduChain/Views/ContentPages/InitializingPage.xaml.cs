namespace eduChain.Views.ContentPages;
using eduChain.Models;
using Firebase;
using Firebase.Auth;
using Plugin.Maui.Audio;

public partial class InitializingPage : ContentPage
{
	FirebaseAuthService firebaseAuthService = FirebaseAuthService.GetInstance();
	FirebaseService	firebaseService = FirebaseService.GetInstance();
	FirebaseAuthClient firebaseAuthClient;
    private readonly IAudioManager audioManager;
	private IAudioPlayer player;


    public InitializingPage(IAudioManager audioManager)
	{
		firebaseAuthClient = firebaseService.GetFirebaseAuthClient();
		this.audioManager = audioManager;
		Preferences.Default.Set("isloaded", "false"); 
		InitializeComponent();
		CheckisLoggedIn();
	}

	protected override async void OnAppearing(){
		Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
		base.OnAppearing();
		player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("ins.mp3"));
		player.Play();
	}
	
	protected override void OnDisappearing(){
		base.OnDisappearing();
		if(player != null)
			player.Dispose();
	}
	public async void CheckisLoggedIn(){
		await Task.Delay(8000);
		NetworkAccess networkAccess = Connectivity.NetworkAccess;
		
		if(networkAccess == NetworkAccess.None)
		{
			if(Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.MacCatalyst || Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.WinUI){
				await Shell.Current.DisplayAlert("No Internet Connection", "Please Connect to the Internet", "OK");
				player.Dispose();
				Application.Current.Quit();	
				return;
			}
		}
		
		if(Preferences.Default.Get("IsLoggedIn",false))
			{		
				try{
				var userCredential = await firebaseAuthClient.SignInWithEmailAndPasswordAsync(Preferences.Default.Get("email", ""), Preferences.Default.Get("password", ""));
				Preferences.Default.Set("firebase_uid", userCredential.User.Uid);
				await Shell.Current.GoToAsync("//homePage");
				}
				catch(FirebaseAuthException){
					await Shell.Current.DisplayAlert("Login Failed", "Your Credentials are now Invalid", "OK");
					Preferences.Default.Clear();
					await Shell.Current.GoToAsync("//loginPage");
				}
				catch(Exception){
					await Shell.Current.DisplayAlert("Login Failed", "An Error Occured", "OK");
					Preferences.Default.Clear();
					await Shell.Current.GoToAsync("//loginPage");
				}
			}	
			else{
				await Shell.Current.GoToAsync("//loginPage");
			}
		}
	}