using eduChain.Models;
using System.Reflection;
using Ipfs;
using Ipfs.Engine;
using Pinata.Client;
using LukeMauiFilePicker;
using CommunityToolkit.Maui.Storage;
using Syncfusion.Maui.TabView;

namespace eduChain.Views.ContentPages;
public partial class IpfsConnectPage : ContentPage
{
    IpfsViewModel ipfsViewModel;
    IPickFile pickedfile;
    PinataClient pinataClient = new PinataClient();
    public IpfsConnectPage()
    {
        InitializeComponent();
        ipfsViewModel = new IpfsViewModel(IPlatformApplication.Current.Services.GetRequiredService<IFileSaver>());
        BindingContext = ipfsViewModel;
    }
   
    
    private async void InitializeTabs(object sender, EventArgs e){
        await ipfsViewModel.ChangeCategory("firstload");
    }
    private async void TabChange(object sender, TabSelectionChangedEventArgs e){
        var selectedItem = e.NewIndex;
        if(selectedItem == 0){
            await ipfsViewModel.ChangeCategory(".jpg");
        }
        else if(selectedItem == 1){
            await ipfsViewModel.ChangeCategory(".mp3");
        }
        else if(selectedItem == 2){
            await ipfsViewModel.ChangeCategory(".mp4");
        }
        else if(selectedItem == 3){
            await ipfsViewModel.ChangeCategory(".pdf");
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

}
