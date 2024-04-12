using eduChain.Models;
using System.Reflection;
using Ipfs.Http;

namespace eduChain.Views.ContentPages;
public partial class IpfsConnectPage : ContentPage 
{
	public IpfsConnectPage()
	{
		InitializeComponent();
	}
    IpfsClient ipfs = new IpfsClient();


    
}

