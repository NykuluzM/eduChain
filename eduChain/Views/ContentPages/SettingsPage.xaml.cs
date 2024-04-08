namespace eduChain.Views.ContentPages;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}
	private void OnSaveUrlClicked(object sender, EventArgs e)
	{
		string url = urlEntry.Text;
		// Save the URL to a persistent storage (e.g., preferences, database)
		// For example, using Preferences:
		Preferences.Set("SchoolWebpageURL", url);

		// Optionally, navigate back to the previous page
	}
}