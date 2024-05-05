using CommunityToolkit.Maui.Views;

namespace eduChain.Views.Popups;
using Camera.MAUI;

public partial class QRReaderPopup : Popup
{
	public QRReaderPopup()
	{
		InitializeComponent();
	}

	private void cameraView_CameraLoaded(object sender, EventArgs e)
	{
		cameraView.Camera = cameraView.Cameras.First(); // Use the first camera
		// Start the camera
		MainThread.BeginInvokeOnMainThread(async () =>
		{
            await cameraView.StartCameraAsync();
        });
		
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