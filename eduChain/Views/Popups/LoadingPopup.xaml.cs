using CommunityToolkit.Maui.Views;
using Plugin.Maui.Audio;

namespace eduChain.Views.Popups;

public partial class LoadingPopup : Popup
{
	private readonly IAudioManager audioManager;
	IAudioPlayer player;
	public LoadingPopup(IAudioManager audioManager)
	{
		InitializeComponent();
		this.audioManager = audioManager;
		player = audioManager.CreatePlayer(FileSystem.OpenAppPackageFileAsync("ins.mp3").Result);
		player.Play();
	}
	
		
	
    public void ClosePopup(){
		player.Stop();
		player.Dispose();
		this.Close();
	}
}