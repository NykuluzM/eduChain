using CommunityToolkit.Maui.Views;

namespace eduChain.Views.Popups
{
	public partial class FormatRegPopup : Popup
	{
		public FormatRegPopup()
		{
			InitializeComponent();
		}


		private void CloseButton_Clicked(object sender, EventArgs e)
		{
			// Close the popup
			this.Close();
		}
	
	}
}