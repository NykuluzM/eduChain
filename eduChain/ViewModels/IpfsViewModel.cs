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
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;
using System;
using System.Drawing; // For handling images

using System.IO;
using System.IO.Compression;
using Syncfusion.Maui.Barcode;
using ZXing.Net.Maui.Readers;
using ZXing.Net.Maui.Controls;
using SkiaSharp;
using ZXing.QrCode;
using ZXing.Net.Maui;
using Pinata.Client.Models;
using Ipfs;

namespace eduChain.ViewModels;

public class IpfsViewModel : ViewModelBase
{
    private bool isRefreshing = false;

    IFileSaver fileSaver;
    public DateTime LastRefreshed { get; set; } = DateTime.Now;
    public string CurrentCategory { get; set; } = "firstload";
    public ICommand ClearCommand { get; }
    public ICommand DownloadCommand { get; }
    public ICommand FileCommand { get; }
    public ICommand VerifyCommand { get; }
    public ICommand CheckCommand { get; }
    public ICommand UnpinCommand { get; }
    public ICommand LoadCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand DecodeQRCommand { get; }

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
                    { DevicePlatform.WinUI, new[] { "*",".jpg",".png",".gif",".docx",".pdf",".xlsx",".xls",".mp3",".mp4",".wav",".mov" } }
                };
    Dictionary<DevicePlatform, IEnumerable<string>> QRFileType = new()
    {
        { DevicePlatform.Android, new[] { "image/.png" } },
        { DevicePlatform.iOS, new[] { "public.png" } },
        { DevicePlatform.MacCatalyst, new[] { "public.png" } },
        { DevicePlatform.WinUI, new[] { ".png" } }
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
        OnPropertyChanged(nameof(FileInfo));

        DownloadCommand = new Command(async () => await DownloadFileByCid(this.Cid[1]));
        VerifyCommand = new Command(async () => await VerifyFile());
        FileCommand = new Command<string>(async (fileId) => await PickFile(fileId));
        ClearCommand = new Command<string>(async (callerId) => await Clear(callerId));
        UnpinCommand = new Command<string>(async (cid) => await UnpinFile());
        RefreshCommand = new Command(async () => await RefreshFiles());
        DecodeQRCommand = new Command(() => DecodeImageButton());
    }
    public async Task<bool> ChangeCategory(string category)
    {
        CategorizedFile.Clear();
        DisplayedFile.Clear();

        switch (category)
        {
            case "firstload":
                Files = new ObservableCollection<FileModel>(await IpfsDatabaseService.Instance.GetAllFilesAsync(UsersProfile.FirebaseId));
                SharedFiles = new ObservableCollection<FileModel>(await IpfsDatabaseService.Instance.GetAllSharedFiles(UsersProfile.FirebaseId));

                LastRefreshed = DateTime.Now;
                CategorizedFile = new ObservableCollection<FileModel>(Files.Where(f => f.FileType == ".jpg" || f.FileType == ".jpeg" || f.FileType == ".png" || f.FileType == ".svg" || f.FileType == ".png" || f.FileType == ".gif"));
                CurrentCategory = "Photos";
                break;
            case "Photos":
                CategorizedFile = new ObservableCollection<FileModel>(Files.Where(f => f.FileType == ".jpg" || f.FileType == ".jpeg" || f.FileType == ".png" || f.FileType == ".svg" || f.FileType == ".png" || f.FileType == ".gif"));
                CurrentCategory = "Photos";
                break;
            case "Audio":
                CategorizedFile = new ObservableCollection<FileModel>(Files.Where(f => f.FileType == ".mp3" || f.FileType == ".wav" || f.FileType == ".m4a"));
                CurrentCategory = "Audio";
                break;
            case "Videos":
                CategorizedFile = new ObservableCollection<FileModel>(Files.Where(f => f.FileType == ".mp4" || f.FileType == ".mov"));
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
    {"Photos", new string[] { ".jpg", ".png", ".svg",".gif" }},
    {"Audio", new string[] { ".mp3", ".wav"}},
    {"Videos", new string[] { ".mp4" ,".mov"}},
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
        if (newFiles.Count() == 0)
        {
            IsRefreshing = false;
            OnPropertyChanged(nameof(IsRefreshing));
            return;
        }
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
    private ObservableCollection<FileModel> _sharedfiles = new ObservableCollection<FileModel>();

    public ObservableCollection<FileModel> SharedFiles
    {
        get { return _sharedfiles; }
        private set { _sharedfiles = value; OnPropertyChanged(nameof(SharedFiles)); }
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
        int toTakePerGroup = Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.Android ||
                             Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.iOS ? 2 : 5;

        int totalFilesAdded = 0; // Track the total number of files added in this run

        // Get all groups from CategorizedFile
        var groups = CategorizedFile
            .GroupBy(cf => cf.FileType);

        foreach (var group in groups)
        {
            int filesAdded = 0; // Track the number of files added for this group

            // Add items to DisplayedFile, ensuring not to exceed 5 files per group in this run
            var itemsToAdd = group.Where(item => !DisplayedFile.Contains(item));
            foreach (var item in itemsToAdd)
            {
                if (filesAdded < toTakePerGroup)
                {
                    DisplayedFile.Add(item);
                    filesAdded++;
                    totalFilesAdded++;
                }
                else
                {
                    break; // Stop adding if reached the maximum limit for this group
                }
            }

            if (totalFilesAdded >= toTakePerGroup * groups.Count())
            {
                break; // No need to continue if we've reached the maximum limit for all groups
            }
        }
    });

    public ICommand ShowLessCommand => new Command(() => {
        int toTakePerGroup = Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.Android ||
                             Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.iOS ? 2 : 5;

        // Get all groups from CategorizedFile
        var groups = CategorizedFile
            .GroupBy(cf => cf.FileType);

        int totalFilesRemoved = 0; // Track the total number of files removed

        foreach (var group in groups)
        {
            int filesRemoved = 0; // Track the number of files removed for this group

            // Remove items from DisplayedFile, ensuring not to exceed 5 files removed per group
            foreach (var item in group)
            {
                if (filesRemoved < toTakePerGroup && totalFilesRemoved < toTakePerGroup * groups.Count())
                {
                    if (DisplayedFile.Contains(item))
                    {
                        DisplayedFile.Remove(item);
                        filesRemoved++;
                        totalFilesRemoved++;
                    }
                }
                else
                {
                    break; // Stop removing if reached the maximum limit for this group or all groups
                }
            }

            if (totalFilesRemoved >= toTakePerGroup * groups.Count())
            {
                break; // No need to continue if we've reached the maximum limit for all groups
            }
        }
    });



    public async Task PickFile(string fileId)
    {

        var fileInfo = await picker.PickFileAsync("Select a file", FileType);
        if (fileInfo == null)
        {
            return;
        }
        if (fileId == "fileforverify") {
            FileInfo[0] = fileInfo;
            // Manually raise the event as Array is a reference type
            OnPropertyChanged(nameof(FileInfo));
        }
        else if (fileId == "fileforupload")
        {
            FileInfo[1] = fileInfo;
            // Manually raise the event as Array is a reference type
            OnPropertyChanged(nameof(FileInfo));
            await Clear("fileforuploadclear");
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
                Cid[1] = "";
                OnPropertyChanged(nameof(Cid));
                FileInfo[1] = null;
                OnPropertyChanged(nameof(FileInfo));
                break;
            case "cidfordownloadclear":
               
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

    public async Task VerifyFile() {


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

        if (isSame == 1) {
            await Shell.Current.DisplayAlert("Verify", "File is same", "OK");
        } else if (isSame == 0) {
            await Shell.Current.DisplayAlert("Verify", "File is not same", "OK");
        }
        else if (isSame == -1) {
            await Shell.Current.DisplayAlert("Verify", "Your targeted CID does not exist in the node", "OK");
        }
        else if (isSame == -2) {
            await Shell.Current.DisplayAlert("Verify", "File upload unsuccessful", "OK");
        }
        else {
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




    private string _recieverUid ;
    public string RecieverUid
    {
        get { return _recieverUid; }
        set
        {
            _recieverUid = value;
            OnPropertyChanged(nameof(RecieverUid));
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
                try {
                    var result = await IpfsDatabaseService.Instance.isPinned(newCid);
                    //if the newly created cid is already in database
                    if (result) {
                        //if the generated cid is the same as the original cid
                        if (newCid == originalCid) {
                            return 1;
                        }
                        //if the generated cid is not the same as the original cid
                        else {
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
                catch (PostgresException e) {
                    return -3;
                }
            }
            else {
                return -2;
            }
        }
    }

    public async Task UploadFileToIpfs() {
        var fileres = FileInfo[1];
        if (fileres == null)
        {
            await Shell.Current.DisplayAlert("Upload", "Please select a file to upload", "OK");
            return;
        }
        if (Cid[1] == null)
        {
            await Shell.Current.DisplayAlert("Upload", "Please enter a CID", "OK");
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
                try {
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    var exist = await IpfsDatabaseService.Instance.isPinned(response.IpfsHash);
                    if (exist)
                    {
                        await Shell.Current.DisplayAlert("Upload", "File already exists in the IPFS node", "OK");
                        return;
                    } else
                    {
                        var IsSuccessful = await IpfsDatabaseService.Instance.InsertPinnedFile(UsersProfile.FirebaseId, UsersProfile.FirebaseId, response.IpfsHash, fileExtension, fileName, Cid[1]);
                        if(IsSuccessful){
                            await Shell.Current.DisplayAlert("Upload", "File uploaded successfully", "OK");
                        } else {
                            await Shell.Current.DisplayAlert("Error", "File Upload Unsuccessful", "OK");
                        }
                        return;

                    }
                }
                catch (PostgresException e) {
                    await this.pinataClient.Pinning.UnpinAsync(response.IpfsHash);
                    await Shell.Current.DisplayAlert("Upload", "File upload unsuccessful", "OK");
                    return;
                }

                finally
                {
                    Cid[1] = string.Empty;
                    FileInfo[1] = null;
                    OnPropertyChanged(nameof(FileInfo));
                }
            }
        }
    }
    private async void DecodeImageButton()
    {
        var pickerResult = await picker.PickFileAsync("Select a QR code image", QRFileType);
        if (pickerResult == null)
        {
            return;
        }
        var bitmap = SKBitmap.Decode(pickerResult.FileResult.FullPath);
        var reader = new ZXing.SkiaSharp.BarcodeReader();
        var result = reader.Decode(bitmap);
        if (result != null)
        {
            var compressedData = Convert.FromBase64String(result.Text);
            using (var compressedStream = new MemoryStream(compressedData))
            using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(gzipStream))
            {
                
                var cidString = streamReader.ReadToEnd();
                 

                var CIDList = cidString.Split('\n').ToList(); // Get the list of CIDs
                if (CIDList.Count < 2)
                {
                    await Shell.Current.DisplayAlert("Error", "Invalid QR Code format", "OK");
                    return;
                }
                var recieverUID = CIDList[0];
                if(recieverUID != UsersProfile.FirebaseId)
                {
                    await Shell.Current.DisplayAlert("Error", "You are not the intended recipient", "OK");
                    return;
                }
                await DownloadFilesByCids(CIDList);
                // Do something with the extracted CIDList
            }
        }
        else
        {
            await Shell.Current.DisplayAlert("Decoded", "No QR code found", "OK");
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
                    if (maxRepeat == 0)
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
            OnPropertyChanged(nameof(Cid));
            // Clean up resources, if necessary
        }
    }
    public async Task DownloadFilesByCids(List<string> cidList)
    {
        if (cidList == null || cidList.Count == 0)
        {
            await Shell.Current.DisplayAlert("Download", "Please provide a list of CIDs", "OK");
            return;
        }
        string formatExpression = null;
        if (DeviceInfo.Platform == DevicePlatform.MacCatalyst) {
            formatExpression = "/";
        }
        else {
            formatExpression = "\\";
        }
        var fileList = await IpfsDatabaseService.Instance.RetrieveFilenames(cidList);
        cidList.Clear();

        using var httpClient = new HttpClient();
        string gatewayUrl;
        string path = null;
        string fileNameBase = null;
        try
        {
            var result = await FolderPicker.PickAsync(default);
            path = result.Folder.Path;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Download", ex.Message, "OK");
            return;
        }
        foreach (var (cid, filename) in fileList)
        {
            gatewayUrl = $"https://gateway.pinata.cloud/ipfs/{cid}";
            try
            {
                var response = await pinataClient.HttpClient.GetAsync(gatewayUrl, HttpCompletionOption.ResponseHeadersRead);
                if (response.IsSuccessStatusCode)
                {
                    string fileName = Path.GetFileName(gatewayUrl); // URI as file name

                    string contentType = response?.Content?.Headers?.ContentType?.MediaType;
                    if (string.IsNullOrEmpty(contentType))
                    {
                        return;
                    }
                    string fileExtension = GetFileExtensionFromContentType(contentType);
                    using (var tempStream = await response.Content.ReadAsStreamAsync())
                    {

                        var currentFileName = $"{path}{formatExpression}{filename}{fileExtension}";
                        if (File.Exists(currentFileName))
                        {
                            for (int i = 0; i < 100; i++)
                            {
                                if (File.Exists($"{path}{formatExpression}{filename}({i}){fileExtension}"))
                                {
                                    continue;
                                }
                                else
                                {
                                    currentFileName = $"{path}{formatExpression}{filename}({i}){fileExtension}";
                                    break;
                                }
                            }
                        }
                        using (var fs = File.Create(currentFileName))
                        {
                            await tempStream.CopyToAsync(fs);
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
        }
        // Display summary message at the end
        await Shell.Current.DisplayAlert("Download", "Files downloaded successfully", "OK");
    }
    private string GetFileExtensionFromContentType(string contentType)
    {
        // You could consider a library like MimeTypes: https://github.com/samuelneff/MimeTypeMap
        var mappings = new Dictionary<string, string> {
            {"image/jpeg", ".jpg"},
            {"image/png", ".png"},
            {"image/gif", ".gif" },
            {"image/svg+xml", ".svg" },
            {"text/plain", ".txt"},
            {"video/mp4", ".mp4" },
            {"audio/mpeg", ".mp3" },
            {"audio/wav", ".wav" },
            {"video/quicktime", ".mov" },
            {"application/json", ".json" },
            {"application/xml", ".xml" },
            {"application/zip", ".zip" },
            {"application/mp3", ".mp3" },
            {"application/mp4", ".mp4" },

            {"application/pdf", ".pdf"},
            {"application/msword", ".doc"},
            {"application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx"},
            {"application/x-ole-storage", ".xls" },
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
            await Shell.Current.DisplayAlert("Unpin", "Please enter a CID", "Yes");
        }
        try
        {
            string gatewayUrl = $"https://gateway.pinata.cloud/ipfs/{this.Cid[2]}";
            var response = await pinataClient.HttpClient.GetAsync(gatewayUrl, HttpCompletionOption.ResponseHeadersRead);
            if (response.Content.Headers == null)
            {
                await Shell.Current.DisplayAlert("Unpin", "File does not exist in the IPFS node", "OK");
                return;
            }
            var result = await IpfsDatabaseService.Instance.isPinned(this.Cid[2]);

            if (result)
            {
                var res = await IsOwnerOfTheFile(this.Cid[2]);
                if(res == false)
                {
                    await Shell.Current.DisplayAlert("Unpin", "You are not the owner of the file", "OK");
                    return;
                }
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
    private async Task<bool> IsOwnerOfTheFile(string cid)
    {
        foreach (var file in Files)
        {
            if (file.CID == cid)
            {

                if (file.Owner == UsersProfile.FirebaseId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }

            // If the CID is not found in the Files collection, return false
            return false;
        }
        return false;
    }
}

