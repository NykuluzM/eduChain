﻿namespace eduChain{
    using System.ComponentModel;
using System.Runtime.CompilerServices;
    using eduChain.Services;
    using eduChain.Models;
    using eduChain.Models.MyProfileModels;
    using Microsoft.Maui;
   

    public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    MyProfileService _myProfileService;
    public ViewModelBase(){
        _myProfileService = MyProfileService.Instance;
        _usersProfile = UsersProfileModel.Instance;
    }
    private UsersProfileModel _usersProfile;
    public UsersProfileModel UsersProfile
    {
        get { return _usersProfile; }
        set
        {
            SetProperty(ref _usersProfile, value);
        }
    }
        public bool hasNavigated { get; set; }

        private FlyoutBehavior _flyoutBehavior = FlyoutBehavior.Flyout;
        public FlyoutBehavior FlyoutBehaviors
        {
            get { return _flyoutBehavior;}
            set
            {
                _flyoutBehavior = value;
                if (value == FlyoutBehavior.Locked)
                {
                    IsMenuPresented = false;
                }
                else if (value == FlyoutBehavior.Flyout)
                {
                    IsMenuPresented = true;
                }
                _flyoutBehavior = value;
                OnPropertyChanged(nameof(FlyoutBehaviors));
            }
        }

        private bool _isMenuPresented = true;
        public bool IsMenuPresented
        {
            get { return _isMenuPresented; }
            set
            {
                _isMenuPresented = value;
                OnPropertyChanged(nameof(IsMenuPresented));
            }
        }

        private OrganizationProfileModel _organizationProfile;
  public OrganizationProfileModel OrganizationProfile
  {
    get { return _organizationProfile; }
    set
    {
      SetProperty(ref _organizationProfile, value);
    }
  }

  private GuardianProfileModel _guardianProfile;
  public GuardianProfileModel GuardianProfile
  {
    get { return _guardianProfile; }
    set
    {
      SetProperty(ref _guardianProfile, value);
    }
  } 
    public async Task LoadUsers(string uid){
        UsersProfile = await _myProfileService.LoadUserAsync(uid, UsersProfileModel.Instance);
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
