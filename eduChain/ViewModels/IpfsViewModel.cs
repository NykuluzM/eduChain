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
using eduChain.Models;
using Npgsql;
using UraniumUI.Material.Controls;
using System.Collections.ObjectModel;
namespace eduChain;

public class IpfsViewModel : ViewModelBase
{
    IFileSaver fileSaver ;
    private string hash = "QmdmfZzdzk7endNWKtgAkn5zF1wnNxVTkU99Z9eNY6761S";
    public ICommand ClearCommand { get; }
    public ICommand UploadCommand { get; }
    public ICommand DownloadCommand { get; }
    public ICommand FileForVerifyCommand { get; }
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
        DownloadCommand = new Command(async () => await DownloadFileByCid());
        VerifyCommand = new Command(async ()=> await VerifyFile());
        UploadCommand = new Command(async () => await UploadFileToIpfs());
        FileForVerifyCommand = new Command(async () => await PickVerifyFile());
        ClearCommand = new Command<string>(async (callerId) => await Clear(callerId));

    }


    public async Task PickVerifyFile()
    {

        var fileInfo = await picker.PickFileAsync("Select a file", FileType);
        if (fileInfo == null)
        {
            return;
        }
        else
        {
            VerifyFileInfo = fileInfo;
        }
      
    }
    public async Task Clear(string callerId)
    {
        switch (callerId)
        {
            case "fileClear":
                VerifyFileInfo = null;
                break;
            default:
                break;
        }
        return;
    }
    public async Task VerifyFile(){

       
        if(Cid == null || Cid == "")
        {
            await Shell.Current.DisplayAlert("Verify", "Please enter a CID", "OK");
            return;
        } 

       
        if(VerifyFileInfo == null)
        {
            await Shell.Current.DisplayAlert("Verify", "Please select a file to verify", "OK");
            return;
        }
        var result = await IpfsDatabaseService.Instance.isPinned(Cid);
        if (result == false)
        {
            await Shell.Current.DisplayAlert("Verify", "Your targeted CID does not exist in the node", "OK");
            return;
        }


        var isSame = await VerifyFileIntegrity(VerifyFileInfo, Cid);

        if(isSame == 1){
            await Shell.Current.DisplayAlert("Verify", "File is same", "OK");
        }else if(isSame == 0){
            await Shell.Current.DisplayAlert("Verify", "File is not same", "OK");
        }
        else if(isSame == -1){
            await Shell.Current.DisplayAlert("Verify", "Your targeted CID does not exist in the node", "OK");
        } 
        else if(isSame == -2){
            await Shell.Current.DisplayAlert("Verify", "File upload unsuccessful", "OK");
        } 
        else{
            await Shell.Current.DisplayAlert("Verify", "Unknown error", "OK");
        }
       
        Cid = "";
        VerifyFileInfo = null;
        return;
    }

    private string _cid;
    public string Cid
    {
        get { return _cid; }
        set
        {
            _cid = value;
            OnPropertyChanged(nameof(Cid));
        }
    }
    private ObservableCollection<KeyValuePair<string, string>> _cidStore =
    new ObservableCollection<KeyValuePair<string, string>>();

   
    private IPickFile _verifyFileInfo;

    public IPickFile VerifyFileInfo
    {
        get { return _verifyFileInfo; }
        set
        {
            _verifyFileInfo = value;
            OnPropertyChanged(nameof(VerifyFileInfo));
        }
    }

   
    public async Task<int> VerifyFileIntegrity(IPickFile file, string originalCid)
    {
     
        using (var fileStream = File.OpenRead(VerifyFileInfo.FileResult.FullPath))
        {
            var existFileFlag = false;
            var existOrigFlag = false;
            var fileContent = new StreamContent(fileStream);       
            var response = await this.pinataClient.Pinning.PinFileToIpfsAsync(content =>
            {
                content.AddPinataFile(fileContent, VerifyFileInfo.FileName);
            });
            if (response.IsSuccess)
            {
                string newCid = response.IpfsHash;
                try{
                var result = await IpfsDatabaseService.Instance.isPinned(newCid);
                //if the newly created cid is already in database
                if(result){
                    //if the generated cid is the same as the original cid
                    if(newCid == originalCid){
                        return 1;
                    }
                    //if the generated cid is not the same as the original cid
                    else{
                        return 0;
                    }
                } 
                //if the generated cid is not in database
                else
                {
                    await this.pinataClient.Pinning.UnpinAsync(newCid);
                    return 0;
                }
                }
                catch(PostgresException e){
                    return -3;
                }
            }
            else{
                return -2;
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
                    try{
                               var fileName = Path.GetFileNameWithoutExtension(filePath);
 
                    await IpfsDatabaseService.Instance.InsertPinnedFile(UsersProfile.FirebaseId,response.IpfsHash,fileExtension,fileName);
                    }
                    catch(PostgresException e){
                        await this.pinataClient.Pinning.UnpinAsync(response.IpfsHash);
                        await Shell.Current.DisplayAlert("Upload", "File upload unsuccessful", "OK");
                        return;
                    }
                    await Shell.Current.DisplayAlert("Upload", "File uploaded successfully", "OK");
                    return;
                }
                await Shell.Current.DisplayAlert("Upload", "File upload failed", "OK");
            }
        }

   public async Task DownloadFileByCid()
    {
    using var httpClient = new HttpClient();
    string gatewayUrl = $"https://gateway.pinata.cloud/ipfs/{this.Cid}";
    try
    {
        var response = await pinataClient.HttpClient.GetAsync(gatewayUrl, HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
            {
                string fileName = Path.GetFileName(gatewayUrl); // URI as file name

                string contentType = response.Content.Headers.ContentType.MediaType;
                string fileExtension = GetFileExtensionFromContentType(contentType); // You'll need a helper function

                var tempFile = await response.Content.ReadAsStreamAsync();
                var path = await fileSaver.SaveAsync(fileName + fileExtension, tempFile);

                await Shell.Current.DisplayAlert("Download", "File downloaded successfully", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Download", "File download failed", "OK");
            }
    }
    catch (Exception ex)
    {
        // Handle errors gracefully, e.g., log the exception or display a user-friendly message
        Console.WriteLine($"Error downloading file: {ex.Message}");
        throw; // Or re-throw for further handling
    }
        finally
        {
            this.Cid = "";
            // Clean up resources, if necessary
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
