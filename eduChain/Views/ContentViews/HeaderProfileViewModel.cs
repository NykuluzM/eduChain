using System.ComponentModel;
using System.Runtime.CompilerServices;

public class HeaderProfileViewModel : INotifyPropertyChanged
{
    // ... implementation of INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;

    private FlyoutBehavior _flyoutBehavior;
    public FlyoutBehavior FlyoutBehavior
    {
        get => _flyoutBehavior;
        set
        {
            _flyoutBehavior = value;
            if(value == FlyoutBehavior.Locked){
                IsMenuPresented = false;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FlyoutBehavior)));
        }
       
    }
    private bool _isMenuPresented;
    public bool IsMenuPresented
    {
        get => _isMenuPresented;
        set
        {
            _isMenuPresented = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsMenuPresented)));
        }
    }

}