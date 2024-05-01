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
    bool firstLoad = true;
    IFileSaver fileSaver ;
    private string hash = "QmdmfZzdzk7endNWKtgAkn5zF1wnNxVTkU99Z9eNY6761S";
    public ICommand ClearCommand { get; }
    public ICommand UploadCommand { get; }
    public ICommand DownloadCommand { get; }
    public ICommand FileCommand { get; }
    public ICommand VerifyCommand { get; } 
    public ICommand CheckCommand { get; }
    public ICommand UnpinCommand { get; }
    public ICommand LoadCommand { get; }

    public event EventHandler InitializationCompleted;


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

    
        DownloadCommand = new Command(async () => await DownloadFileByCid());
        VerifyCommand = new Command(async ()=> await VerifyFile());
        UploadCommand = new Command(async () => await UploadFileToIpfs());
        FileCommand = new Command<string>(async (fileId) => await PickFile(fileId));
        ClearCommand = new Command<string>(async (callerId) => await Clear(callerId));
        UnpinCommand = new Command<string>(async (cid) => await UnpinFile());

    }
    public async Task ChangeCategory(string category)
    {
        if(category == "firstload"){
            Files = new ObservableCollection<FileModel>(await IpfsDatabaseService.Instance.GetAllFilesAsync(UsersProfile.FirebaseId));
            CategorizedFile = new ObservableCollection<FileModel>(Files.Where(f => f.FileType == ".jpg"));
        }
        else if(category == ".jpg"){
            CategorizedFile = new ObservableCollection<FileModel>(Files.Where(f => f.FileType == ".jpg"));
        }
        else if(category == ".mp3"){
            CategorizedFile = new ObservableCollection<FileModel>(Files.Where(f => f.FileType == ".mp3"));
        }
        else if(category == ".mp4"){
            CategorizedFile = new ObservableCollection<FileModel>(Files.Where(f => f.FileType == ".mp4"));
        }
        else if(category == ".png"){
            CategorizedFile = new ObservableCollection<FileModel>(Files.Where(f => f.FileType == ".png"));
        }
        else if(category == ".pdf"){
            CategorizedFile = new ObservableCollection<FileModel>(Files.Where(f => f.FileType == ".pdf"));
        }
        else if(category == ".docx"){
            CategorizedFile = new ObservableCollection<FileModel>(Files.Where(f => f.FileType == ".docx"));
        }
        else{
            CategorizedFile = new ObservableCollection<FileModel>(Files);
        }
    }
    public string SelectedCategory { get; set; }

    private ObservableCollection<FileModel> _files = new ObservableCollection<FileModel>();

    public ObservableCollection<FileModel> Files
    {
        get { return _files; }
        private set { _files = value; OnPropertyChanged(nameof(Files)); }
    }
    ObservableCollection<FileModel> _categorizedFile = new ObservableCollection<FileModel>();
    public ObservableCollection<FileModel> CategorizedFile {
                                get {
                                        return _categorizedFile; 
                                } 
                                set { 
                                    _categorizedFile = value;
                                    OnPropertyChanged(nameof(CategorizedFile));
                                } 
    }
    
    private async Task LoadFilesByCategoryAsync(string category)
    {
        List<FileModel> files = await IpfsDatabaseService.Instance.GetByFileType(category, UsersProfile.FirebaseId);
        Files = new ObservableCollection<FileModel>(files); // Update collection and notify UI
    }
    public async Task PickFile(string fileId)
    {

        var fileInfo = await picker.PickFileAsync("Select a file", FileType);
        if (fileInfo == null)
        {
            return;
        }
        if(fileId == "fileforverify") { 
            FileInfo[0] = fileInfo;
            // Manually raise the event as Array is a reference type
            OnPropertyChanged(nameof(FileInfo)); 
        }
        else if(fileId == "fileforupload")
        {
            FileInfo[1] = fileInfo;
            // Manually raise the event as Array is a reference type
            OnPropertyChanged(nameof(FileInfo));
        }

    }
    public async Task Clear(string callerId)
    {
        switch (callerId)
        {
            case "fileforverifyclear":
                FileInfo[0] = null;
                OnPropertyChanged(nameof(FileInfo));
                break;
            case "cidforverifyclear":
                Cid[0] = "";
                OnPropertyChanged(nameof(Cid));
                break;
            case "fileforuploadclear":
                FileInfo[1] = null;
                OnPropertyChanged(nameof(FileInfo));
                break;
            case "cidfordownloadclear":
                Cid[1] = "";
                OnPropertyChanged(nameof(Cid));
                break;
            case "cidforunpinclear":
                Cid[2] = "";
                OnPropertyChanged(nameof(Cid));
                break;
            default:
                break;
        }
        return;
    }
   
    public async Task VerifyFile(){


        if (Cid[0] == null || Cid[0] == "")
        {
            await Shell.Current.DisplayAlert("Verify", "Please enter a CID", "OK");
            return;
        }


        if (FileInfo[0] == null)
        {
            await Shell.Current.DisplayAlert("Verify", "Please select a file to verify", "OK");
            return;
        }
        var result = await IpfsDatabaseService.Instance.isPinned(Cid[0]);
        if (result == false)
        {
            await Shell.Current.DisplayAlert("Verify", "Your targeted CID does not exist in the node", "OK");
            return;
        }


        var isSame = await VerifyFileIntegrity(FileInfo[0], Cid[0]);

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

        Cid[0] = "";
        OnPropertyChanged(nameof(Cid));
        FileInfo[0] = null;
        OnPropertyChanged(nameof(FileInfo));

        return;
    }

    private string[] _cid = new String[3];
    public string[] Cid
    {
        get { return _cid; }
        set
        {
            _cid = value;
            OnPropertyChanged(nameof(Cid));
        }
    }
    

   
  


    private IPickFile[] _fileInfo = new IPickFile[2];
    public IPickFile[] FileInfo
    {
        get { return _fileInfo; }
        set
        {
            _fileInfo = value;
            OnPropertyChanged(nameof(FileInfo));
        }
    }

    public async Task<int> VerifyFileIntegrity(IPickFile file, string originalCid)
    {
     
        using (var fileStream = File.OpenRead(FileInfo[0].FileResult.FullPath))
        {
            var existFileFlag = false;
            var existOrigFlag = false;
            var fileContent = new StreamContent(fileStream);       
            var response = await this.pinataClient.Pinning.PinFileToIpfsAsync(content =>
            {
                content.AddPinataFile(fileContent, FileInfo[0].FileName);
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
    public async Task<IPickFile> SelectFile()
    {
        var fileres = await picker.PickFileAsync("Select a file", FileType);
        return fileres;
    }
   private async Task UploadFileToIpfs(){
        var fileres = FileInfo[1];
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
                            var exist = await IpfsDatabaseService.Instance.isPinned(response.IpfsHash);
                            if (exist)
                            {
                                await Shell.Current.DisplayAlert("Upload", "File already exists in the IPFS node", "OK");
                                return;
                            } else
                            {
                                await IpfsDatabaseService.Instance.InsertPinnedFile(UsersProfile.FirebaseId, response.IpfsHash, fileExtension, fileName);
                                await Shell.Current.DisplayAlert("Upload", "File uploaded successfully", "OK");
                                return;

                            }
                        }
                    catch(PostgresException e){
                        await this.pinataClient.Pinning.UnpinAsync(response.IpfsHash);
                        await Shell.Current.DisplayAlert("Upload", "File upload unsuccessful", "OK");
                        return;
                    }
                
                    finally
                    {
                        FileInfo[1] = null;
                        OnPropertyChanged(nameof(FileInfo));
                    }
            }
            }
        }

   public async Task DownloadFileByCid()
    {
        if (string.IsNullOrEmpty(this.Cid[1]))
        {
            await Shell.Current.DisplayAlert("Download", "Please enter a CID", "OK");
            return;
        }

    using var httpClient = new HttpClient();
    string gatewayUrl = $"https://gateway.pinata.cloud/ipfs/{this.Cid[1]}";
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
                await Shell.Current.DisplayAlert("Download", "File download failed, The File does not exist or its unavailable", "OK");
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
            this.Cid[1] = "";
            OnPropertyChanged(nameof(Cid));
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
    private async Task UnpinFile()
    {
        if (string.IsNullOrEmpty(this.Cid[2]))
        {
            await Shell.Current.DisplayAlert("Unpin","Please enter a CID","Yes");
        }
        try
        {
            string gatewayUrl = $"https://gateway.pinata.cloud/ipfs/{this.Cid[2]}";
            var response = await pinataClient.HttpClient.GetAsync(gatewayUrl, HttpCompletionOption.ResponseHeadersRead);
            if (!response.IsSuccessStatusCode)
            {
                return;
            }
            var result = await IpfsDatabaseService.Instance.isPinned(this.Cid[2]);

            if (result)
            {
                await IpfsDatabaseService.Instance.UnpinFile(this.Cid[2]);
                await this.pinataClient.Pinning.UnpinAsync(this.Cid[2]);
                await Shell.Current.DisplayAlert("Unpin", "File unpinned successfully", "OK");
                return;
            }
            else
            {
                await Shell.Current.DisplayAlert("Unpin", "File does not exist in the IPFS node", "OK");
                return;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Unpin", ex.ToString(), "OK");
            throw;
        }
        finally
        {
            this.Cid[2] = "";
            OnPropertyChanged(nameof(Cid));
        }
    }   
}
