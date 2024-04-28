using eduChain.Models;
using System.Reflection;
using Ipfs;
using Ipfs.Engine;
using Pinata.Client;

namespace eduChain.Views.ContentPages;
public partial class IpfsConnectPage : ContentPage 
{
    PinataClient pinataClient = new PinataClient();
	public IpfsConnectPage()
	{
		InitializeComponent();
        BindingContext = new IpfsViewModel();

	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Start the IPFS client
    }
    

 



}

