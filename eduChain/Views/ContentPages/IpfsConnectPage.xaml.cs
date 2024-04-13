using eduChain.Models;
using System.Reflection;
using Ipfs.Http;
using Ipfs;
using Ipfs.Engine;
namespace eduChain.Views.ContentPages;
public partial class IpfsConnectPage : ContentPage 
{
	public IpfsConnectPage()
	{
		InitializeComponent();
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Start the IPFS client
        await StartIPFSAsync();
    }
  public async Task StartIPFSAsync()
{
    IpfsEngine ipfsEng = new IpfsEngine();
    IpfsClient ipfs = new IpfsClient();
    try
    {
        // Create an instance of IpfsClient

        // Get node information asynchronously
        var nodeInfo = await ipfs.IdAsync();
        await Shell.Current.DisplayAlert("Node Info", nodeInfo.ToString(), "OK");

        // Get and display the IPFS client version asynchronously
        var clientVersion = await ipfs.VersionAsync();
        await Shell.Current.DisplayAlert("IPFS client version", clientVersion.ToString(), "OK");
    }
    catch (Exception ex)
    {
        // Handle any exceptions that may occur
        await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
    }
    finally
    {
        // Make sure to shut down the IPFS client
        await ipfs.ShutdownAsync();
    }
}
 



}

