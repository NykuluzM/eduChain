using CommunityToolkit.Maui.Views;
using FirebaseAdmin.Auth;
using Plugin.Maui.Audio;

namespace eduChain.Views.Popups;

public partial class ChangePasswordPopup : Popup
{
	public ChangePasswordPopup()
	{
		InitializeComponent();
	}
	
	
	private void Close(Object sender,EventArgs e){
		this.Close();
	}
	private async void Change(Object sender,EventArgs e){
		if(newPassword.Text == null || newPassword.Text.Length < 6){
			Close();
			return;
		}
		await ChangePassword(newPassword.Text);
		Close();
		return;
	}
	 protected async Task ChangePassword(string newPassword){
        await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.UpdateUserAsync(new UserRecordArgs
        {
            Uid = Preferences.Default.Get("firebase_uid", string.Empty),
            Password = newPassword 
        });

    }
   
}