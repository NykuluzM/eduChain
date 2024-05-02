using CommunityToolkit.Maui.Views;
using Plugin.Maui.Audio;

namespace eduChain.Views.Popups;

public partial class MPPopup : Popup
{
    public MPPopup(string filename,string cid)
    {
        InitializeComponent();

        string gatewayUrl = $"https://gateway.pinata.cloud/ipfs/{cid}";

        playerHolder.Source = gatewayUrl;
        labelHolder.Text = filename;
    }



    public void ClosePopup(Object o, EventArgs e)
    {
        playerHolder.Stop();
        playerHolder.Dispose();
       
        playerHolder.Handler?.DisconnectHandler();
        this.Close();
    }
}