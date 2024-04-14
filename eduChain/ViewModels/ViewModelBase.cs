namespace eduChain{
    using System.ComponentModel;
using System.Runtime.CompilerServices;
    using eduChain.Services;
    using eduChain.Models;

    public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    bool _isProfileLoaded;
    MyProfileService _myProfileService;
    public ViewModelBase(){
        _myProfileService = MyProfileService.Instance;
    }
     private MyProfileModel _profile;
    public MyProfileModel Profile
        {
            get { return _profile; }
            set
            {
                _profile = value;
                OnPropertyChanged(nameof(Profile));            }
        }
    
      public async Task LoadProfileAsync(string uid)
        {
            if (!_isProfileLoaded) // Check if the profile is not loaded
            {
                Profile = await _myProfileService.GetUserProfileAsync(uid, MyProfileModel.Instance);
                _isProfileLoaded = true; // Set the flag to indicate profile is loaded
            }
        }
    
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(storage, value))
        {
            return false;
        }

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

}
