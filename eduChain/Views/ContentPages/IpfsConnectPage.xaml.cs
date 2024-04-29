using eduChain.Models;
using System.Reflection;
using Ipfs;
using Ipfs.Engine;
using Pinata.Client;
using CommunityToolkit.Maui.Storage;

namespace eduChain.Views.ContentPages;
public partial class IpfsConnectPage : ContentPage 
{
    PinataClient pinataClient = new PinataClient();
	public IpfsConnectPage()
	{
		InitializeComponent();
        BindingContext = new IpfsViewModel(IPlatformApplication.Current.Services.GetRequiredService<IFileSaver>());

	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Start the IPFS client
    }
    

 



}

