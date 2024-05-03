using CommunityToolkit.Maui.Views;
using Plugin.Maui.Audio;

namespace eduChain.Views.Popups;

public partial class DOCPopup : Popup
{
    public DOCPopup(string filename,string cid)
    {
        InitializeComponent();

        string gatewayUrl = $"https://gateway.pinata.cloud/ipfs/{cid}";

        labelHolder.Text = filename;
		webHolder.Source = gatewayUrl;
		
    }



    public void ClosePopup(Object o, EventArgs e)
    {
        this.Close();
    }
}