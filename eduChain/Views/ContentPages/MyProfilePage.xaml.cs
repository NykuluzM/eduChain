using SkiaSharp;
namespace eduChain.Views.ContentPages{
	public partial class MyProfilePage : ContentPage
	{
		public MyProfilePage()
		{
			InitializeComponent();
		}
		 private async void SelectImageButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Select an image from the device
                var file = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Select an Image"
                });

                if (file == null)
                    return;

                // Load the selected image using SkiaSharp
                using (var stream = await file.OpenReadAsync())
                {
                    SKBitmap bitmap = SKBitmap.Decode(stream);
                    SKImage image = SKImage.FromBitmap(bitmap);
                    SelectedImage.Source = ImageSource.FromStream(() => image.Encode().AsStream());
                }

                // Implement cropping logic here using SkiaSharp and display the cropped image
                // Example: allow the user to define a cropping area and crop the image
                // using SKBitmap.Resize() or other cropping methods
            }
            catch (Exception ex)
            {
            Application.Current?.MainPage?.DisplayAlert("No Connection", ex.Message, "OK");
                // Handle errors
            }
        }
	}
}