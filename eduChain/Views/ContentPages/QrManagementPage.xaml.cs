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
		var hasValues = await viewModel.InitializeDataAsync();
		if (!hasValues)
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