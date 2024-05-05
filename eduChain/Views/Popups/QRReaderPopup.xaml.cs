using CommunityToolkit.Maui.Views;

namespace eduChain.Views.Popups;

public partial class QRReaderPopup : Popup
{
	public QRReaderPopup()
	{
		InitializeComponent();
		barcodeReader.Options = new ZXing.Net.Maui.BarcodeReaderOptions
		{
			Formats = ZXing.Net.Maui.BarcodeFormat.QrCode,
			AutoRotate = true,
			TryInverted = true,
		};

		barcodeReader.CameraLocation = ZXing.Net.Maui.CameraLocation.Front;

	}
	private async void barcodeReader_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
	{
		// Handle the detected barcodes
		var first = e.Results?.FirstOrDefault();
		if (first is null){
			return;
		}
		await Dispatcher.DispatchAsync(() =>
		{
			// Display the detected barcode
			Shell.Current.DisplayAlert("Barcode Detected", first.Value, "OK");
		});
	}

	public void ClosePopup(Object o, EventArgs e)
	{
		this.Close();
	}
}