using System.ComponentModel;
using CommunityToolkit.Maui.Core;
using eduChain.Views.Popups;

namespace eduChain.ViewModels.PopupViewModels;

public class LoadingPopupViewModel : ViewModelBase
{
    private readonly IPopupService _popupService;
    public LoadingPopupViewModel(IPopupService popupService)
    {
        this._popupService = popupService;
    }

    public void ShowLoadingPopup()
    {
        _popupService.ShowPopup<LoadingPopup>();
    }

}
