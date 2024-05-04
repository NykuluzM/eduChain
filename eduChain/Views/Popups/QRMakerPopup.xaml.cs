
namespace eduChain.Views.Popups;
using System.Text;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Views;
using Syncfusion.Maui.Core;
using Syncfusion.Maui.Core.Internals;
public partial class QRMakerPopup : Popup
{
	IFileSaver fileSaver = IPlatformApplication.Current.Services.GetRequiredService<IFileSaver>();
	public QRMakerPopup(byte[] data)
	{
		InitializeComponent();
		bcode.Value = Convert.ToBase64String(data);  // Convert compressed data to Base64

	}
	public async void SaveAsImage(Object o, EventArgs e)
	{
		// Save the data as an image
		var stream = await bcode.GetStreamAsync(ImageFileFormat.Png);
		try{
			await fileSaver.SaveAsync("QRCode.png",stream);
		}
		catch(Exception ex){
			await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
		
		}
		finally{
			this.Close();
		}
	}
	public void ClosePopup(Object o, EventArgs e)
    {
        this.Close();
    }
}