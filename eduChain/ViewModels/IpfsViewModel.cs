using System.ComponentModel;
using System.Security.Cryptography;
using System.Windows.Input;
using LukeMauiFilePicker;
using Pinata.Client;

namespace eduChain;

public class IpfsViewModel : ViewModelBase
{

    public ICommand DownloadCommand { get; }
    public ICommand VerifyCommand { get; } 
      Dictionary<DevicePlatform, IEnumerable<string>> FileType = new()
                {
                    { DevicePlatform.Android, new[] { "*/*" } },
                    { DevicePlatform.iOS, new[] { "*" } },
                    { DevicePlatform.MacCatalyst, new[] { "*" } },
                    { DevicePlatform.WinUI, new[] { "*" } }
                };
    public IpfsViewModel()
    {
         var config = new Config
        {    
            ApiKey = "809a9fb6a4270d8db032",
        };
        PinataClient pinataClient = new PinataClient(config);

        string cid = "QmaoNjCucWMB7w3PcytfNumFAG3xwqAHz47NTYgDwznnd6"; // Replace with your actual CID
        string gatewayUrl = $"https://gateway.pinata.cloud/ipfs/{cid}";
        DownloadCommand = new Command(async () => await DownloadFileByCid(cid));
        VerifyCommand = new Command(async () => await VerifyFile());
    }
    public async Task VerifyFile(){
        var file = await picker.PickFileAsync("Select a file", FileType);

        //await VerifyFileIntegrity(gatewayUrl, cid);
    }
    public async Task<bool> VerifyFileIntegrity(string downloadedFilePath, string originalCid)
{
    using (var fileStream = File.OpenRead(downloadedFilePath))
    {
        using (var sha256 = SHA256.Create())
        {
            var hashBytes = await sha256.ComputeHashAsync(fileStream);
            var calculatedCid = Convert.ToHexString(hashBytes); // Adapt conversion for your CID format

            return calculatedCid == originalCid;
        }
    }
}

    async Task<string> DownloadFileByCid(string cid)
    {
    using var httpClient = new HttpClient();
    string gatewayUrl = $"https://gateway.pinata.cloud/ipfs/{cid}";

    try
    {
        var response = await httpClient.GetAsync(gatewayUrl, HttpCompletionOption.ResponseHeadersRead);
        if (response.IsSuccessStatusCode)
        {
            string fileName = Path.GetFileName(gatewayUrl); // Extract filename from URL
            string downloadPath = Path.Combine(Path.GetTempPath(), fileName); // Use temporary path

            using (var fileStream = File.Create(downloadPath))
            {
                await response.Content.CopyToAsync(fileStream);
                await Shell.Current.DisplayAlert("Download", "File downloaded successfully", "OK");
                return downloadPath; // Return the downloaded file path
            }
        }
        else
        {
            throw new Exception($"Error downloading file: {response.StatusCode}");
        }
    }
    catch (Exception ex)
    {
        // Handle errors gracefully, e.g., log the exception or display a user-friendly message
        Console.WriteLine($"Error downloading file: {ex.Message}");
        throw; // Or re-throw for further handling
    }
    }
}
