using eduChain.Models;
using eduChain.ViewModels;
using Google.Protobuf.WellKnownTypes;
namespace eduChain.Views.ContentPages;

public partial class QrManagementPage : ContentPage
{
	QrViewModel viewModel;
	public QrManagementPage()
	{
		BindingContext = viewModel = new QrViewModel();
		InitializeComponent();
		
	}
	protected override async void OnAppearing()
	{
        base.OnAppearing();
		viewModel.QrList = await QrDatabaseService.Instance.GetMyQrList(viewModel.UsersProfile.FirebaseId);
		if (viewModel.QrList == null || viewModel.QrList.Count == 0)
		{
			EmptyLab.IsVisible = true;
			Qrs.IsVisible = false;
			Header.IsVisible = false;
		}
		else
		{
			EmptyLab.IsVisible = false;
			Qrs.IsVisible = true;
			Header.IsVisible = true;
		}
    }
}