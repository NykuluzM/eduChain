using SkiaSharp;
using LukeMauiFilePicker;

namespace eduChain.Views.ContentPages{
	public partial class MyProfilePage : ContentPage
	{
        readonly IFilePickerService picker;
         Dictionary<DevicePlatform, IEnumerable<string>> FileType = new()
                {
                    { DevicePlatform.Android, new[] { "image/*" } },
                    { DevicePlatform.iOS, new[] { "public.image" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.jpeg", "public.png" } },
                    { DevicePlatform.WinUI, new[] { ".jpg", ".png" } }
                };

		public MyProfilePage()
		{
			InitializeComponent();
            picker = IPlatformApplication.Current.Services.GetRequiredService<IFilePickerService>();

		}
		private async void SelectImageButton_Clicked(object sender, EventArgs e)
{
    try
    {
        // Select an image from the device
        var file = await picker.PickFileAsync("Select a file", FileType);
        if (file is null) { return; }
        // Set the image source aof SelectedImage
        using (var stream = await file.OpenReadAsync())
        {
            SKBitmap bitmap = SKBitmap.Decode(stream);
            SKImage image = SKImage.FromBitmap(bitmap);
            SelectedImage.Source = ImageSource.FromStream(() => image.Encode().AsStream());
        }
    }
    catch (Exception ex)
    {
        Application.Current?.MainPage?.DisplayAlert("No Connection", ex.Message, "OK");
        // Handle errors
    }
}
	}
}