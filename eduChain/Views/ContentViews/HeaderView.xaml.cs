using eduChain;

namespace eduChain.Views.ContentViews;

public partial class HeaderView : ContentView
{
	private AppShellViewModel _viewModel;
	public FlyoutBehavior ShellFlyoutBehavior 
    {
        get => Shell.Current.FlyoutBehavior;  
        set 
        {
			 
                if(value == FlyoutBehavior.Locked)	
				{
					menuButton.IsVisible = false;
				}	
                OnPropertyChanged(nameof(Shell.Current.FlyoutBehavior));
            
            CheckVisibility(value);
        }
    }

	public HeaderView()
	{
		InitializeComponent();
		_viewModel = AppShellViewModel.Instance;
		BindingContext = _viewModel;

	}
	private void Menu(object sender, EventArgs e)
	{
			Shell.Current.FlyoutIsPresented = true;
	}
	private void CheckVisibility(FlyoutBehavior value)
	{
		if (value == FlyoutBehavior.Disabled)
		{
			 Shell.Current.FlyoutIsPresented = false;
		}
	}
}