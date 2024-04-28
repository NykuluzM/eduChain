using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Windows.Input;
using Flurl.Http.Content;
using LukeMauiFilePicker;
using MimeMapping;
using Pinata.Client;
using Ipfs.Engine;
using Flurl.Http;
namespace eduChain;

public class IpfsViewModel : ViewModelBase
{
    private string hash = "QmcVLCMg5q9p7MmUE7nuiVFwmiDNMXJrhheBy7dHZUnKDg";
    public ICommand UploadCommand { get; }
    public ICommand DownloadCommand { get; }
    public ICommand VerifyCommand { get; } 
    public ICommand CheckCommand { get; }
    PinataClient pinataClient;
      Dictionary<DevicePlatform, IEnumerable<string>> FileType = new()
                {
                    { DevicePlatform.Android, new[] { "*/*" } },
                    { DevicePlatform.iOS, new[] { "public.*" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.*" } },
                    { DevicePlatform.WinUI, new[] { "*" } }
                };
    public IpfsViewModel()
    {
         var config = new Config
        {    
            ApiKey = "b7aaa5289c05382e0563",
            ApiSecret = "f7b28b0095efdcb761e848cfd0150f7677674abcd4ab0a7ea4295b6abc0507a3"
        };
        pinataClient = new PinataClient(config);

        string cid = "QmVnNCU2vTSeEPK3XqHvekawGohfSjqAcKUHEJfgpr8JEg"; // Replace with your actual CID
        string gatewayUrl = $"https://gateway.pinata.cloud/ipfs/{cid}";
        DownloadCommand = new Command(async () => await DownloadFileByCid(cid));
        VerifyCommand = new Command(async () => await VerifyFile());
        UploadCommand = new Command(async () => await UploadFileToIpfs());
        CheckCommand = new Command(async () => await CheckList());
    }
    public async Task VerifyFile(){
        var file = await picker.PickFileAsync("Select a file", FileType);
        if (file == null)
        {
            return;
        }
        string path = file.FileResult.FullPath;
        var isSame = await VerifyFileIntegrity(file, hash);
        if(isSame){
            await Shell.Current.DisplayAlert("Verify", "File is same", "OK");
        }else{
            await Shell.Current.DisplayAlert("Verify", "File is not same", "OK");
        }
    }
    public async Task CheckList(){
            
    }
    public async Task<bool> VerifyFileIntegrity(IPickFile file, string originalCid)
    {

        using (var fileStream = File.OpenRead(file.FileResult.FullPath))
        {
            var existFlag = false;
            var fileContent = new StreamContent(fileStream);

            var List = await pinataClient.Data.PinList();
            if (List.Rows.Any(item => item.IpfsPinHash == originalCid))
            {
                existFlag = true;
                // Targeted CID exists - no need to pin and verify
            }
            var response = await this.pinataClient.Pinning.PinFileToIpfsAsync(content =>
            {
                content.AddPinataFile(fileContent, file.FileName);
            });
            if (response.IsSuccess)
            {
                string newCid = response.IpfsHash;
                if(originalCid == newCid){
                    return true;
                }
                else{
                    if(existFlag){
                        existFlag = false;
                        return false;
                    }
                    await pinataClient.Pinning.UnpinAsync(response.IpfsHash);
                    return false;
                }
            }
            else{
                return false;
            }
        }
    }

   private async Task UploadFileToIpfs(){
        var fileres = await picker.PickFileAsync("Select a file", FileType);
        if (fileres == null)
        {
            return; // Handle canceled selection
        }
        string filePath = fileres.FileResult.FullPath;
        string fileExtension = Path.GetExtension(filePath);
       try
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                var fileContent = new StreamContent(fileStream);
                //File Processing and Uploading to IPFS
                if (!string.IsNullOrEmpty(fileExtension))
                {
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(MimeUtility.GetMimeMapping(fileExtension));
                }
            
                var response = await this.pinataClient.Pinning.PinFileToIpfsAsync(content =>
                {
                    content.AddPinataFile(fileContent, fileres.FileName);
                });
                //"F8FD40F55D00F38D4BAC2FA62E0552993DE6CC442699178CF6CF466C77D5655C"
                if (response.IsSuccess)
                {
                    await Shell.Current.DisplayAlert("Upload", "File uploaded successfully", "OK");
                    return;
                }
                await Shell.Current.DisplayAlert("Upload", "File upload failed", "OK");
            }
        }
        catch (Exception e)
        {
            await Shell.Current.DisplayAlert("Upload", e.Message, "OK");
        }
   }
   private async Task<string> DownloadFileByCid(string cid)
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
