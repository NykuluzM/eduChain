using eduChain.Models;
using System.Reflection;
using Ipfs;
using Ipfs.Engine;
using Pinata.Client;
using CommunityToolkit.Maui.Storage;

namespace eduChain.Views.ContentPages;
public partial class IpfsConnectPage : ContentPage 
{
    IpfsViewModel ipfsViewModel;
    PinataClient pinataClient = new PinataClient();
	public IpfsConnectPage()
	{
		InitializeComponent();
        ipfsViewModel = new IpfsViewModel(IPlatformApplication.Current.Services.GetRequiredService<IFileSaver>());
        BindingContext = ipfsViewModel;
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Start the IPFS client
    }
    

    private void Toggle(object sender, EventArgs e)
    {
        var s = (UraniumUI.Material.Controls.CheckBox)sender;
        
        switch (s.ClassId)
        {
            case "Verify":
                if(s.IsChecked)
                {
                    VerifyLayout.IsVisible = true;
                }
                else
                {
                    VerifyLayout.IsVisible = false;
                }   
                break;
            case "Download":
                if(s.IsChecked)
                {
                    DownloadLayout.IsVisible = true;
                }
                else
                {
                    DownloadLayout.IsVisible = false;
                }
                break;
            case "Upload":
                if(s.IsChecked)
                {
                    UploadLayout.IsVisible = true;
                }
                else
                {
                    UploadLayout.IsVisible = false;
                }
                break;  
        }

    }

    private async void Submit(object sender, EventArgs e)
    {
        var s = (Button)sender;
        if(s.ClassId == "verifyClick")
        {
            ipfsViewModel.Cid = verifyCid.Text;
            await ipfsViewModel.VerifyFile();
            verifyCid.Text = "";
        }
        else if(s.ClassId == "downloadClick")
        {
            ipfsViewModel.Cid = downloadCid.Text;
            await ipfsViewModel.DownloadFileByCid();
            downloadCid.Text = "";
        }
    }

    private void Button_Clicked(object sender, EventArgs e)
    {

    }
}

