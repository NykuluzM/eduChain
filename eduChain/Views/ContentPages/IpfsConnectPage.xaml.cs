using eduChain.Models;
using System.Reflection;
using Ipfs;
using Ipfs.Engine;
using Pinata.Client;
using LukeMauiFilePicker;
using CommunityToolkit.Maui.Storage;
using Syncfusion.Maui.TabView;
using Syncfusion.Maui.DataSource.Extensions;
using Syncfusion.Maui.ListView;
using eduChain.Views.Popups;
using CommunityToolkit.Maui.Views;
using System.IO.Compression;
using CommunityToolkit.Maui.Alerts;
using eduChain.ViewModels;
using Camera.MAUI;
using System.Collections.ObjectModel;
using Syncfusion.Maui.PullToRefresh;
namespace eduChain.Views.ContentPages;
public partial class IpfsConnectPage : ContentPage
{
    IpfsViewModel ipfsViewModel;
    IPickFile pickedfile;
    PinataClient pinataClient = new PinataClient();
    SearchBar searchBar;
    private MPPopup mediaPopup;
    private bool firstLoad = true;
    public IpfsConnectPage()
    {
        InitializeComponent();
        ipfsViewModel = new IpfsViewModel(IPlatformApplication.Current.Services.GetRequiredService<IFileSaver>());
        BindingContext = ipfsViewModel;
        MyDocumentsList.SelectionBackground = Colors.Khaki;
        ShowMore(this, null);
    }
   
    
    private async void InitializeTabs(object sender, EventArgs e){
        if(firstLoad == true){
            firstLoad = false;
           var res = await ipfsViewModel.ChangeCategory("firstload");    
           if(res == false){
                ShowMoreFiles.IsVisible = false;
                ShowLessFiles.IsVisible = false;
                NoFiles.IsVisible = true;
           } else {
               ShowMore(this, null);
           }
        }
    }
    private async void ToggleMyFiles(object sender, EventArgs e){
        var s = (Button)sender;
        if(s.ClassId == "Show"){
            tabGrid.IsVisible = true;
            fManagerShow.IsVisible = false;
            fManagerHide.IsVisible = true;
        }
        else{
            tabGrid.IsVisible = false;
            fManagerHide.IsVisible = false;
            fManagerShow.IsVisible = true;  
        }
    }
    private async void QRImagePrompt(object sender, EventArgs e){
     
    }
    private void Filter(object sender, EventArgs e){
    
        searchBar = (SearchBar)sender;
        SfListView parent = null;
        switch(searchBar.ClassId){
            case "DocumentsSearch":
                parent = MyDocumentsList;
                break;
            case "PhotosSearch":
                parent = MyPhotosList;
                break;
            case "AudioSearch":
                parent = MyAudioList;
                break;
            case "VideosSearch":
                parent = MyVideosList;
                break;
            case "AllSearch":
                parent = MyAllList;
                break;
            case "SharedSearch":
                parent = SharedFilesList;
                break;
        }
        if(parent.DataSource != null){
            parent.DataSource.Filter = SearchByName;
            parent.DataSource.RefreshFilter();
        }
    }
    private bool SearchByName(object obj){
        if(searchBar.Text == null || searchBar == null)
        {
            return true;
        }
        var fileInfo = obj as FileModel;
        if (fileInfo == null)
        {
            return false; // Not a FileInfo object, don't display
        }

        // Check directly against the FileInfo object
        return fileInfo.FileName.ToLower().Contains(searchBar.Text.ToLower()) ||
           fileInfo.CID.ToLower().Contains(searchBar.Text.ToLower()) || fileInfo.FileType.ToLower().Equals(searchBar.Text.ToLower());
    }
    private async void QR_clicked(Object sender,EventArgs e){
        var s = (Button)sender;
 
        SfListView parent = null;
        var selectedItems = MyPhotosList.SelectedItems;
        switch(s.ClassId){
            case "DocumentsQR":
                parent = MyDocumentsList;
                break;
            case "PhotosQR":
                parent = MyPhotosList;
                break;
            case "AudioQR":
                parent = MyAudioList;
                break;
            case "VideosQR":
                parent = MyVideosList;
                break;
            case "AllQR":
                parent = MyAllList;
                break;
        }
        selectedItems = parent.SelectedItems;
        if(selectedItems.Count == 0){
            var toast = Toast.Make("No files selected");
            await toast.Show();
            return;
        }
        
        var CIDList = new List<string>();
        foreach(var item in selectedItems){
            var file = item as FileModel;
            CIDList.Add(file.CID);
        }
         // Compression using GZipStream
        using (var memoryStream = new MemoryStream()) 
        {
            using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
            using (var streamWriter = new StreamWriter(gzipStream)) 
            {
                // For plain text CIDs separated by newlines
                var cidString = string.Join("\n", CIDList); 
                streamWriter.Write(cidString); 
            }

            var compressedData = memoryStream.ToArray(); // Get the compressed data as a byte array
            // Do something with the compressedData (e.g., store it, encode in QR code)
            var qrPopup = new QRMakerPopup(compressedData);
            this.ShowPopup(qrPopup);
        }

        parent.SelectedItems.Clear();
    }
    private async void TabChange(object sender, TabSelectionChangedEventArgs e){
        ShowLessFiles.IsVisible = false;
        ShowMoreFiles.IsVisible = true;
        var selectedItem = e.NewIndex;
        var hasValues = false;
        switch (selectedItem)
        {
            case 0:
                hasValues = await ipfsViewModel.ChangeCategory("Photos");
                totalfilecount.IsVisible = false;
                categoryfilecount.IsVisible = true;
                break;
            case 1:
                hasValues = await ipfsViewModel.ChangeCategory("Audio");
                totalfilecount.IsVisible = false;
                categoryfilecount.IsVisible = true;
                break;
            case 2:
                hasValues = await ipfsViewModel.ChangeCategory("Videos");
                totalfilecount.IsVisible = false;
                categoryfilecount.IsVisible = true;
                break;
            case 3:
                hasValues = await ipfsViewModel.ChangeCategory("Documents");
                totalfilecount.IsVisible = false;
                categoryfilecount.IsVisible = true;

                break;
            case 4:
                totalfilecount.IsVisible = true;
                categoryfilecount.IsVisible = false;
                break;
        }
        if (!hasValues)
        {
            ShowMoreFiles.IsVisible = false;
            ShowLessFiles.IsVisible = false;
        } else
        {
            ShowMoreFiles.IsVisible = true;
        }
        if (!categoryfilecount.IsVisible)
        {
            if (ipfsViewModel.Files.Count == 0)
            {
                NoFiles.IsVisible = true;

                return;
            }
            else
            {
                NoFiles.IsVisible = false;
            }
            ShowMoreFiles.IsVisible = false;
            ShowLessFiles.IsVisible = false;
            return;
        }
        ShowMore(this, null);
    }

   private void ToggleSharedFiles(object sender, EventArgs e)
    {
        var s = (Button)sender;
        if(s.ClassId == "Show")
        {
            ShowShared.IsVisible = false;
            HideShared.IsVisible = true;
            SharedFiles.IsVisible = true;
            
        }
        else
        {
            ShowShared.IsVisible = true;
            HideShared.IsVisible = false;
            SharedFiles.IsVisible = false;
        }
    }
    private void Toggle(object sender, EventArgs e)
    {
        if (sender is Button)
        {
            var s = (Button)sender;
            if (s.ClassId == "ShowAll")
            {
                VerifyToggle.IsChecked = true;
                DownloadToggle.IsChecked = true;
                UploadToggle.IsChecked = true;
                UnpinToggle.IsChecked = true;
                VerifyLayout.IsVisible = true;
                DownloadLayout.IsVisible = true;
                UploadLayout.IsVisible = true;
                UnpinLayout.IsVisible = true;
                ShowToggle.IsVisible = false;
                HideToggle.IsVisible = true;
            }

            else
            {
                VerifyToggle.IsChecked = false;
                DownloadToggle.IsChecked = false;
                UploadToggle.IsChecked = false;
                UnpinToggle.IsChecked = false;
                VerifyLayout.IsVisible = false;
                DownloadLayout.IsVisible = false;
                UploadLayout.IsVisible = false;
                UnpinLayout.IsVisible = false;
                ShowToggle.IsVisible = true;
                HideToggle.IsVisible = false;
            }

        }
        else
        {
            var s = (UraniumUI.Material.Controls.CheckBox)sender;


            switch (s.ClassId)
            {
                case "Verify":
                    if (s.IsChecked)
                    {
                        VerifyLayout.IsVisible = true;

                    }
                    else
                    {
                        VerifyLayout.IsVisible = false;
                    }
                    break;
                case "Download":

                    if (s.IsChecked)
                    {
                        DownloadLayout.IsVisible = true;
                    }
                    else
                    {
                        DownloadLayout.IsVisible = false;
                    }
                    break;
                case "Upload":
                    if (s.IsChecked)
                    {
                        UploadLayout.IsVisible = true;
                    }
                    else
                    {
                        UploadLayout.IsVisible = false;
                    }
                    break;
                case "Unpin":
                    if (s.IsChecked)
                    {
                        UnpinLayout.IsVisible = true;
                    }
                    else
                    {
                        UnpinLayout.IsVisible = false;
                    }
                    break;


            }
            if (VerifyToggle.IsChecked && DownloadToggle.IsChecked && UploadToggle.IsChecked && UnpinToggle.IsChecked)
            {
                ShowToggle.IsVisible = false;
                HideToggle.IsVisible = true;
            }
            else if (!VerifyToggle.IsChecked && !DownloadToggle.IsChecked && !UploadToggle.IsChecked && !UnpinToggle.IsChecked)
            {
                ShowToggle.IsVisible = true;
                HideToggle.IsVisible = false;
            }

        }

    }
    private void ShowMore(object sender, EventArgs e)
    {
        NoFiles.IsVisible = false;

        if(ipfsViewModel.CategorizedFile.Count == 0)
        {
            ShowMoreFiles.IsVisible = false;
            ShowLessFiles.IsVisible = false;
            NoFiles.IsVisible = true;
            return;
        }
        ShowLessFiles.IsVisible = true;
        if(ipfsViewModel.DisplayedFile.Count == ipfsViewModel.CategorizedFile.Count)
        {
            ShowMoreFiles.IsVisible = false;
        } else
        {
            ShowMoreFiles.IsVisible = true;
        }
    }

    private void ShowLess(object sender, EventArgs e)
    {
        if(ipfsViewModel.DisplayedFile.Count == 0)
        {
            ShowLessFiles.IsVisible = false;
            ShowMoreFiles.IsVisible = true;
            NoFiles.IsVisible = true;   
        } else
        {
            ShowLessFiles.IsVisible = true;
            ShowMoreFiles.IsVisible = true;
        }
    } 
    private void Check_Refreshed(object sender, EventArgs e)
    {
        var s = (SfPullToRefresh)sender;
        if(s.ClassId == "RefreshAll")
        {
            if(ipfsViewModel.Files.Count == 0)
            {
                NoFiles.IsVisible = true;
                return;
            }
            else
            {
                NoFiles.IsVisible = false;
            }
            ShowMoreFiles.IsVisible = false;
            ShowLessFiles.IsVisible = false;
        } else
        {
            if(ipfsViewModel.CategorizedFile.Count == 0)
            {
                NoFiles.IsVisible = true;
                ShowMoreFiles.IsVisible = false;
                ShowLessFiles.IsVisible = false;
                return;
            }
            else
            {
                NoFiles.IsVisible = false;
                ShowLessFiles.IsVisible = true;
            }
           
        }
    }
    private void Preview(object sender, EventArgs e)
    {
        var s = (Button)sender;

        string rawCid = s.ClassId;
        string filename = s.CommandParameter.ToString();
        string filetype = s.ImageSource.ToString();
        if(filetype == "File: .mp3" || filetype == "File: .wav")
        {
            mediaPopup = new MPPopup(filename, rawCid);
            this.ShowPopup(mediaPopup);

        } else if(filetype == "File: .pdf")
        {
            var mediaPopup = new DOCPopup(filename, rawCid);
            this.ShowPopup(mediaPopup);
        } else if(filetype == "File: .mp4" || filetype == "File: .mov")
        {
            var mediaPopup = new MPPopup(filename, rawCid);
            this.ShowPopup(mediaPopup);
        } else if(filetype == "File: .jpg" || filetype == "File: .png" || filetype == "File: .gif")
        {
            var imagePopup = new IMAGEPopup(filename, rawCid);
            this.ShowPopup(imagePopup);
        }
    }


    
    private async void Push_Download(object sender, EventArgs e)
    {
        var s = (Button)sender;
        string rawCid = s.ClassId;
        await ipfsViewModel.DownloadFileByCid(rawCid);
    }
    
    private async void OnCopyButtonClicked(object sender, EventArgs e)
    {
        var s = (Button)sender;
        string textToCopy =  s.ClassId; // Get the text you want to copy (filename or downloaded content)
        await CopyTextToClipboard(textToCopy);
        string message = "CID Copied to clipboard";
        var toast = Toast.Make(message);
        await toast.Show();
    }
    public async Task CopyTextToClipboard(string text)
{
  if (!string.IsNullOrEmpty(text))
  {
    await Clipboard.Default.SetTextAsync(text);
  }
}
}
