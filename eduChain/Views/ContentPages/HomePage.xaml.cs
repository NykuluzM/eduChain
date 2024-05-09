
using CommunityToolkit.Maui.Views;
using eduChain.Models;
using eduChain.Services;
using eduChain.Views.Popups;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Alerts;


using IAudioManager = Plugin.Maui.Audio.IAudioManager;
using Syncfusion.Maui.TabView;
using Syncfusion.Maui.Core.Carousel;
using System.Collections.ObjectModel;
namespace eduChain.Views.ContentPages{
	public partial class HomePage : ContentPage
	{
        private string currenttabaffiliationrequest = "organizationaffiliation";
        private string currenttabaffiliation = "organizationaffiliation";
        private HomePageViewModel homePageViewModel;
		private LoadingPopup loadingPopup;
		public HomePage()
		{
			InitializeComponent();
			homePageViewModel = new HomePageViewModel();
            BindingContext = homePageViewModel;

        }
        private async void Load(){
			await LoadProfile();
            await homePageViewModel.InitializeAsync();

        }
        protected override async void OnAppearing()
		{
			base.OnAppearing();
			
			if(Preferences.Default.Get("isloaded", String.Empty) == "false")
			{
				Load();
				Preferences.Default.Set("isloaded", "true");
			}
			Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
        }
        private async void Tab1Change(object sender, TabSelectionChangedEventArgs e)
        {
            var selectedItem = e.NewIndex;
            switch (selectedItem)
            {
                case 0:
                    OrgUIDButton.IsVisible = true;
                    StudUIDButton.IsVisible = false;
                    OrgUIDRequest.IsVisible = true;
                    StudUIDRequest.IsVisible = false;
                    currenttabaffiliation = "organizationaffiliation";
                    break;
                case 1:
                    OrgUIDButton.IsVisible = false;
                    StudUIDButton.IsVisible = true;
                    OrgUIDRequest.IsVisible = false;
                    StudUIDRequest.IsVisible = true;
                    currenttabaffiliation = "studentaffiliation";
                    break;
            }
        }
        private async void Tab2Change(object sender, TabSelectionChangedEventArgs e)
        {
            var selectedItem = e.NewIndex;
            switch (selectedItem)
            {
                case 0:
                    currenttabaffiliationrequest = "organizationaffiliation";
                    break;
                case 1:
                    currenttabaffiliationrequest = "studentaffiliation";
                    break;
            }
        }


        private async void RequestAffiliation(object sender, EventArgs e)
        {
            var s = (Button)sender;
            switch (s.ClassId)
            {
                case "OrgAff":
                    var res = await homePageViewModel.UIDVerify(OrgUIDRequest.Text, "org");
                    if (res == false)
                    {
                        await DisplayAlert("Error", "Organization not found, Please Enter a correct UID", "OK");
                    }
                    else
                    {
                        await homePageViewModel.SendAffRequest(OrgUIDRequest.Text);
                        await homePageViewModel.InitializeAsync();

                    }
                    OrgUIDRequest.Text = "";
                    break;
                case "StudAff":
                    var ress = await homePageViewModel.UIDVerify(StudUIDRequest.Text, "stud");
                    if (ress == false)
                    {
                        await DisplayAlert("Error", "Student not found, Please Enter a correct UID", "OK");
                    }
                    else
                    {
                        await homePageViewModel.SendAffRequest(StudUIDRequest.Text);
                        await homePageViewModel.InitializeAsync();

                    }
                    StudUIDRequest.Text = "";
                    break;

            }

        }
       
        private async void RequestsHandler(object sender, EventArgs e)
        {
            var s = (Button)sender;
            string decision = s.Text;
            List<AffiliationsModel> selectedItems = new List<AffiliationsModel>();
            if (currenttabaffiliationrequest == "organizationaffiliation")
            {
                selectedItems = homePageViewModel.AffiliationRequestsOrganizations
                                    .Where(org => org.IsSelected)
                                    .ToList();
            }
            else
            {
                selectedItems = homePageViewModel.AffiliationRequestsStudents
                                    .Where(stud => stud.IsSelected)
                                    .ToList();
            }
            if(selectedItems.Count == 0)
            {
                await DisplayAlert("Error", "Please select an item", "OK");
                return;
            }
            var selectedItemsObservable = new ObservableCollection<object>(selectedItems);
            switch (decision)
            {
                case "Accept":
                    await homePageViewModel.AcceptAffiliations(selectedItemsObservable);
                    await Shell.Current.DisplayAlert("Success", "Accepted Requests", "OK");

                    break;
                case "Reject":
                    await homePageViewModel.RejectAffiliations(selectedItemsObservable);
                    await Shell.Current.DisplayAlert("Success", "Rejected Requests", "OK");
                    break;
            }
            await homePageViewModel.InitializeAsync();
        }


        

        private async void RemoveAffiliations(object sender, EventArgs e)
        {
            if (currenttabaffiliation == "organizationaffiliation")
            {
                if (affiorg.SelectedRows == null || affiorg.SelectedRows.Count < 1)
                {
                    await DisplayAlert("Error", "Please select an item", "OK");
                    return;
                }
                else
                {
                    await homePageViewModel.RemoveAffiliations(affiorg.SelectedRows);
                    await homePageViewModel.InitializeAsync();
                }
            }
            else
            {
                if (affistud.SelectedRows == null || affistud.SelectedRows.Count < 1)
                {
                    await DisplayAlert("Error", "Please select an item", "OK");
                    return;
                }
                else
                {
                    await homePageViewModel.RemoveAffiliations(affistud.SelectedRows);
                    await homePageViewModel.InitializeAsync();
                }
            }
        }

        private async Task LoadProfile(){
			try{
				homePageViewModel.IsLoading = true;
	
			    var audioConnection = IPlatformApplication.Current.Services.GetRequiredService<IAudioManager>();
				loadingPopup = new LoadingPopup(audioConnection);
				this.ShowPopup(loadingPopup);
				await homePageViewModel.LoadUsers(Preferences.Default.Get("firebase_uid", string.Empty));
				await Task.Delay(1000);
				//await homePageViewModel.LoadProfileAsync(Preferences.Default.Get("firebase_uid", string.Empty));
				//await Shell.Current.DisplayAlert("Success", "Profile Loaded", null);
				//await Shell.Current.Navigation.PopModalAsync();
			}
			catch{

			}
			finally{
                AppShell appShell = (App.Current as App).MainPage as AppShell;
                Shell.Current.FlyoutIsPresented = true;
				if(DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.Android)
				{
					appShell.TriggerLayout("expanded");
					Shell.Current.FlyoutIsPresented = false;
				} else
				{
                    appShell.TriggerLayout("collapsed");

                }
                await Task.Delay(50);
                loadingPopup.ClosePopup();
				homePageViewModel.IsLoading = false;
			}
		}
		
	}
}