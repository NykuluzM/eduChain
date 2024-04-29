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
using CommunityToolkit.Maui.Storage;
using System.Text;
using System.Text.RegularExpressions;
namespace eduChain;

public class IpfsViewModel : ViewModelBase
{
    IFileSaver fileSaver ;
    private string hash = "QmdmfZzdzk7endNWKtgAkn5zF1wnNxVTkU99Z9eNY6761S";
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
                    { DevicePlatform.WinUI, new[] { "*",".jpg",".png",".docx",".pdf" } }
                };
    public IpfsViewModel(IFileSaver fileSaver)
    {
        this.fileSaver = fileSaver;
        fileSaver = DependencyService.Get<IFileSaver>();
         var config = new Config
        {    
            ApiKey = "b7aaa5289c05382e0563",
            ApiSecret = "f7b28b0095efdcb761e848cfd0150f7677674abcd4ab0a7ea4295b6abc0507a3"
        };
        pinataClient = new PinataClient(config);

        string cid = "QmdmfZzdzk7endNWKtgAkn5zF1wnNxVTkU99Z9eNY6761S"; // Replace with your actual CID
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
                    } else {
                        await pinataClient.Pinning.UnpinAsync(response.IpfsHash);
                        return false;
                    }
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
   private async Task DownloadFileByCid(string cid)
    {
    using var httpClient = new HttpClient();
    string gatewayUrl = $"https://gateway.pinata.cloud/ipfs/{cid}";
    try
    {
        var response = await pinataClient.HttpClient.GetAsync(gatewayUrl, HttpCompletionOption.ResponseHeadersRead);
        if (response.IsSuccessStatusCode)
        {
            string fileName = Path.GetFileName(gatewayUrl); // URI as file name

            string contentType = response.Content.Headers.ContentType.MediaType;
            string fileExtension = GetFileExtensionFromContentType(contentType); // You'll need a helper function

            var tempFile = await response.Content.ReadAsStreamAsync(); 
            using var reader = new BinaryReader(tempFile);
            using var stream = new MemoryStream(reader.ReadBytes((int)tempFile.Length));
            try{
            var path = await fileSaver.SaveAsync(fileName + fileExtension,stream);
            }catch(Exception e){
                await Shell.Current.DisplayAlert("Download", e.Message, "OK");
            }
            await Shell.Current.DisplayAlert("Download", "File downloaded successfully", "OK");  
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
    private string GetFileExtensionFromContentType(string contentType)
    {
        // You could consider a library like MimeTypes: https://github.com/samuelneff/MimeTypeMap
        var mappings = new Dictionary<string, string> {
            {"image/jpeg", ".jpg"},
            {"image/png", ".png"},
            {"text/plain", ".txt"},
            {"application/pdf", ".pdf"},
            {"application/msword", ".doc"},
            {"application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx"},
            {"application/vnd.ms-excel", ".xls"},
            {"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ".xlsx"},
            {"application/vnd.ms-powerpoint", ".ppt"},
            {"application/vnd.openxmlformats-officedocument.presentationml.presentation", ".pptx"}
            // ... add more mappings
        };

        return mappings.TryGetValue(contentType, out var extension) ? extension : ".bin"; // Default to ".bin" if not found
    }
}
