using CommunityToolkit.Maui.Views;

namespace eduChain.Views.Popups;
using Camera.MAUI;

public partial class QRReaderPopup : Popup
{
	public QRReaderPopup()
	{
		InitializeComponent();
        cameraView.CamerasLoaded += cameraView_CameraLoaded;
	}

	private async void cameraView_CameraLoaded(object sender, EventArgs e)
	{
		// Start the camera
		cameraView.Camera = cameraView.Cameras.First();
		
			var result = await cameraView.StartCameraAsync();
			if(result == null){
				// Handle the error
				Shell.Current.DisplayAlert("Error", "Failed to start the camera", "OK");
			}
		
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