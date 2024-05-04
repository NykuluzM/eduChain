using CommunityToolkit.Maui.Views;
using Plugin.Maui.Audio;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Storage;
using System.Text;
using Syncfusion.Pdf;
using Pinata.Client;

namespace eduChain.Views.Popups;

public partial class DOCPopup : Popup
{
	string gatewayUrl;
    public DOCPopup(string filename,string cid)
    {
        InitializeComponent();
    	IFileSaver fileSaver ;
        gatewayUrl = $"https://gateway.pinata.cloud/ipfs/{cid}";
        labelHolder.Text = filename ;
		var pdfViewer = new PdfViewerViewModel();
		BindingContext = pdfViewer;
		pdfViewer.SetPdfDocumentStream(gatewayUrl);
    }

 		


    public void ClosePopup(Object o, EventArgs e)
    {
        this.Close();
    }
}