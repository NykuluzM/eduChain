using CommunityToolkit.Maui.Views;

namespace eduChain.Views.Popups;

public partial class IMAGEPopup : Popup
{
	public IMAGEPopup(string filename,string cid)
	{
		InitializeComponent();
		string gatewayUrl = $"https://gateway.pinata.cloud/ipfs/{cid}";
		imageHolder.Source = gatewayUrl;
		labelHolder.Text = filename;
	}
	    public void ClosePopup(Object o, EventArgs e)
    {
        this.Close();
    }
}