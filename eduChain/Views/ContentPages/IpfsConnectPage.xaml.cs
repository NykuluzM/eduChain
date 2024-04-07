using eduChain.Models;
using System.Reflection;

namespace eduChain.Views.ContentPages;
public partial class IpfsConnectPage : ContentPage 
{
	public IpfsConnectPage()
	{
		InitializeComponent();
	}

private async Task UploadFileToPinata_Success()
{
	await DisplayAlert("Uploading", "Upload button clicked", "OK");
    // Arrange

string projectDirectory = "IPFS//test_file.txt";

	byte[] fileData = null;
    try
    {
		 File.ReadAllBytes(projectDirectory);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error reading file: {ex.Message}");
        await DisplayAlert("Error", $"Error reading file: {ex.Message}", "OK");
        return;
    }
    string fileName = "test_file.txt";

    try
    {
        Console.WriteLine("Starting file upload to Pinata...");

        // Act
        var response = await PinataClientProvider.Instance.Pinning.PinFileToIpfsAsync(multipartContent => {
            using var stream = new MemoryStream(fileData);
            multipartContent.AddFile("file", stream, fileName);
        }, new Pinata.Client.PinataMetadata { Name = fileName });

        Console.WriteLine("File upload to Pinata completed.");

        // Assert
        if (response.IsSuccess)
        {
            Console.WriteLine($"File uploaded to Pinata with IPFS hash: {response.IpfsHash}");
            await DisplayAlert("Success", $"File uploaded to Pinata with IPFS hash: {response.IpfsHash}", "OK");
        }
        else
        {
            Console.WriteLine($"Failed to upload file to Pinata: {response.Message}");
            await DisplayAlert("Error", $"Failed to upload file to Pinata: {response.Message}", "OK");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception: {ex.Message}");
        await DisplayAlert("Error", $"Exception: {ex.Message}", "OK");
    }
}
 

        private async void OnUploadButtonClicked(object sender, EventArgs e)
        {
			await Application.Current.MainPage.DisplayAlert("Upload", "Upload button clicked", "OK");
			await UploadFileToPinata_Success();
        }
}

