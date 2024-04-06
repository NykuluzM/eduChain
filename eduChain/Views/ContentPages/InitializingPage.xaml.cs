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
		InitializeComponent();
		CheckisLoggedIn();
	}

	protected override async void OnAppearing(){
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
		await Task.Delay(16000);
		if(Preferences.Default.Get("IsLoggedIn",false) == true)
			{		
				try{
				await firebaseAuthClient.SignInWithEmailAndPasswordAsync(Preferences.Default.Get("email", ""), Preferences.Default.Get("password", ""));
				await Shell.Current.GoToAsync("//homePage");
				}
				catch(FirebaseAuthException){
					await Shell.Current.DisplayAlert("Login Failed", "Your Credentials are now Invalid", "OK");
				}
				catch(Exception){
					await Shell.Current.DisplayAlert("Login Failed", "An Error Occured", "OK");
				}
			}	
			else{
				await Shell.Current.GoToAsync("//homePage");
			}
		}
	}