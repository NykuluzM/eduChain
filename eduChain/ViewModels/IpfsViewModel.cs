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
    private bool isRefreshing = false;
   
    IFileSaver fileSaver ;
    private string hash = "QmdmfZzdzk7endNWKtgAkn5zF1wnNxVTkU99Z9eNY6761S";
    public DateTime LastRefreshed { get; set; } = DateTime.Now;
    public string CurrentCategory { get; set; } = "firstload";
    public ICommand ClearCommand { get; }
    public ICommand UploadCommand { get; }
    public ICommand DownloadCommand { get; }
    public ICommand FileCommand { get; }
    public ICommand VerifyCommand { get; } 
    public ICommand CheckCommand { get; }
    public ICommand UnpinCommand { get; }
    public ICommand LoadCommand { get; }
    public ICommand RefreshCommand { get; }
    public event EventHandler InitializationCompleted;

    public bool IsRefreshing
    {
        get => isRefreshing;
        set
        {
            isRefreshing = value;
            OnPropertyChanged(nameof(IsRefreshing));
        }
    }   

    PinataClient pinataClient;
      Dictionary<DevicePlatform, IEnumerable<string>> FileType = new()
                {
                    { DevicePlatform.Android, new[] { "*/*" } },
                    { DevicePlatform.iOS, new[] { "public.*" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.*" } },
                    { DevicePlatform.WinUI, new[] { "*",".jpg",".png",".docx",".pdf",".xlsx",".xls",".mp3",".mp4",".wav" } }
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

    
        DownloadCommand = new Command(async () => await DownloadFileByCid(this.Cid[1]));
        VerifyCommand = new Command(async ()=> await VerifyFile());
        UploadCommand = new Command(async () => await UploadFileToIpfs());
        FileCommand = new Command<string>(async (fileId) => await PickFile(fileId));
        ClearCommand = new Command<string>(async (callerId) => await Clear(callerId));
        UnpinCommand = new Command<string>(async (cid) => await UnpinFile());
        RefreshCommand = new Command(async () => await RefreshFiles());   
    }
    public async Task<bool> ChangeCategory(string category)
    {
        CategorizedFile.Clear();
        DisplayedFile.Clear();

        switch (category)
        {
            case "firstload":
                Files = new ObservableCollection<FileModel>(await IpfsDatabaseService.Instance.GetAllFilesAsync(UsersProfile.FirebaseId));
                LastRefreshed = DateTime.Now;
                CategorizedFile = new ObservableCollection<FileModel>(Files.Where(f => f.FileType == ".jpg" || f.FileType == ".png" || f.FileType == ".svg" || f.FileType == ".png" || f.FileType == ".gif"));
                CurrentCategory = "Photos";
                break;
            case "Photos":
                CategorizedFile = new ObservableCollection<FileModel>(Files.Where(f => f.FileType == ".jpg" || f.FileType == ".png" || f.FileType == ".svg" || f.FileType == ".png" || f.FileType == ".gif"));
                CurrentCategory = "Photos";
                break;
            case "Audio":
                CategorizedFile = new ObservableCollection<FileModel>(Files.Where(f => f.FileType == ".mp3" || f.FileType == ".wav" || f.FileType == ".m4a"));
                CurrentCategory = "Audio";  
                break;
            case "Videos":
                CategorizedFile = new ObservableCollection<FileModel>(Files.Where(f => f.FileType == ".mp4"));
                CurrentCategory = "Videos";
                break;
            case "Documents":
                CategorizedFile = new ObservableCollection<FileModel>(Files.Where(f => f.FileType == ".pdf" || f.FileType == ".docx" || f.FileType == ".xlsx" || f.FileType == ".xls"));
                CurrentCategory = "Documents";
                break;
        }
        LoadMoreCommand.Execute(null);

        if (CategorizedFile.Count == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private Dictionary<string, string[]> categoryFileTypes = new Dictionary<string, string[]>
{
    {"Photos", new string[] { ".jpg", ".png", ".svg" }},
    {"Audio", new string[] { ".mp3", ".wav", ".m4a" }},
    {"Videos", new string[] { ".mp4" }},
    {"Documents", new string[] { ".pdf", ".docx", ".xlsx", ".xls" }}
};
    private bool FileBelongsToCategory(FileModel file, string category)
    {
        if (categoryFileTypes.ContainsKey(category))
        {
            return categoryFileTypes[category].Contains(file.FileType);
        }
        else
        {
            return false; // Or handle an 'Others' category
        }
    }
    public async Task RefreshFiles()
    {
        IsRefreshing = true;
        var updatedFileList = await IpfsDatabaseService.Instance.RefreshFilesAsync(UsersProfile.FirebaseId, LastRefreshed);

        // Filter for new files (assuming CID is your unique identifier)
        var newFiles = updatedFileList.Where(newFile => !Files.Any(existingFile => existingFile.CID == newFile.CID));

        // Add new files to both collections
        foreach (var file in newFiles)
        {
            if (FileBelongsToCategory(file, CurrentCategory))
            {
                CategorizedFile.Add(file);
                DisplayedFile.Add(file);
            }
            Files.Add(file);
        }
        LastRefreshed = DateTime.Now;
        IsRefreshing = false;
        OnPropertyChanged(nameof(IsRefreshing));
    }
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
    ObservableCollection<FileModel> _displayFile = new ObservableCollection<FileModel>();

    public ObservableCollection<FileModel> DisplayedFile
    {
        get
        {
            return _displayFile;
        }
        set
        {

            OnPropertyChanged(nameof(DisplayedFile));
        }
    }

    public ICommand LoadMoreCommand => new Command(() => {
        var currentCount = DisplayedFile.Count;
        int toTake = 0;

        if (Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.Android || Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.iOS)
        {
            toTake = 2;
        } else
        {
            toTake = 5;
        }
        var itemsToLoad = CategorizedFile.Skip(currentCount).Take(toTake);
        if(itemsToLoad == null || itemsToLoad.Count() == 0)
        {
            return;
        }
        foreach (var item in itemsToLoad)
        {
            DisplayedFile.Add(item);
        }

    });
    public ICommand ShowLessCommand => new Command(() => {
        var currentCount = DisplayedFile.Count;
        int toTake = 0;
        if (Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.Android || Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.iOS)
        {
            toTake = 2;
        }
        else
        {
            toTake = 5;
        }
        var itemsToDeLoad = CategorizedFile.Skip(currentCount - toTake).Take(toTake);
        if (itemsToDeLoad == null || itemsToDeLoad.Count() == 0)
        {
            return;
        }
        foreach (var item in itemsToDeLoad)
        {
            DisplayedFile.Remove(item);
        }
    });

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
   
   private async Task UploadFileToIpfs(){
        var fileres = FileInfo[1];
        if(fileres == null)
        {
            await Shell.Current.DisplayAlert("Upload", "Please select a file to upload", "OK");
            return;
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

   public async Task DownloadFileByCid(string cid)
    {
        if (string.IsNullOrEmpty(cid))
        {
            await Shell.Current.DisplayAlert("Download", "Please enter a CID", "OK");
            return;
        }

    using var httpClient = new HttpClient();
    string gatewayUrl = $"https://gateway.pinata.cloud/ipfs/{cid}";
    try
    {
            var maxRepeat = 5;
            while (maxRepeat > 0)
            {


                var response = await pinataClient.HttpClient.GetAsync(gatewayUrl, HttpCompletionOption.ResponseHeadersRead);
                if (response.IsSuccessStatusCode)
                {
                    string fileName = Path.GetFileName(gatewayUrl); // URI as file name

                    string contentType = response.Content.Headers.ContentType.MediaType;
                    string fileExtension = GetFileExtensionFromContentType(contentType); // You'll need a helper function

                    var tempFile = await response.Content.ReadAsStreamAsync();
                    var path = await fileSaver.SaveAsync(fileName + fileExtension, tempFile);
                    if (path.IsSuccessful)
                    {
                        await Shell.Current.DisplayAlert("Download", "File downloaded successfully", "OK");
                    }
                    return;

                }
                else
                {
                    maxRepeat--;
                    if(maxRepeat == 0)
                    {
                        await Shell.Current.DisplayAlert("Download", "File download failed, The File does not exist or its unavailable", "OK");
                    }
                }
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
            {"application/json", ".json" },
            {"application/xml", ".xml" },
            {"application/zip", ".zip" },
            {"application/mp3", ".mp3" },
            {"application/mp4", ".mp4" },
     
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
