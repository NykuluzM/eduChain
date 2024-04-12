using Plugin.Maui.Audio;
namespace eduChain.Views.ContentPages;

public partial class LoadingOnePage : ContentPage
{
	private readonly IAudioManager audioManager;
	private IAudioPlayer player;
	public LoadingOnePage(IAudioManager audioManager)
	{
		InitializeComponent();
	}
	protected override async void OnAppearing(){
		Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
		base.OnAppearing();
		player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("gikumot.mp3"));
		player.Play();
	}
}