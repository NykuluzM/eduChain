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
            await ipfsViewModel.ChangeCategory("firstload");    
        }
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
            case 5:
                totalfilecount.IsVisible = true;
                categoryfilecount.IsVisible = false;
                break;
        }
        if (!hasValues)
        {
            ShowMoreFiles.IsVisible = false;
        } else
        {
            ShowMoreFiles.IsVisible = true;
        }
        ShowMore(this, null);
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
    }
    public async Task CopyTextToClipboard(string text)
{
  if (!string.IsNullOrEmpty(text))
  {
    await Clipboard.Default.SetTextAsync(text);
  }
}
}
