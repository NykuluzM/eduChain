using CommunityToolkit.Maui.Views;
using Plugin.Maui.Audio;

namespace eduChain.Views.Popups;

public partial class MPPopup : Popup
{
    private readonly IAudioManager audioManager;
    IAudioPlayer player;
    public MPPopup(string cid)
    {
        InitializeComponent();

        string gatewayUrl = $"https://gateway.pinata.cloud/ipfs/{cid}";

        playerHolder.Source = gatewayUrl;
    }

    private string _media;
    public string Media
    {
        get => _media;
        set
        {
            _media = value;
            OnPropertyChanged();
        }
    }



    public void ClosePopup(Object o, EventArgs e)
    {
        playerHolder.Stop();
        playerHolder.Dispose();
       
        playerHolder.Handler?.DisconnectHandler();
        this.Close();
    }
}