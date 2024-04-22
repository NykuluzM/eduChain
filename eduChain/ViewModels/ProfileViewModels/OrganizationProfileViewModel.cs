using eduChain.Models.MyProfileModels;
namespace eduChain.ViewModels.ProfileViewModels;

public class OrganizationProfileViewModel : BaseProfileViewModel
{
    private static OrganizationProfileModel _organizationProfile;
    public OrganizationProfileModel OrganizationProfile
    {
        get { return _organizationProfile; }
        set
        {
        SetProperty(ref _organizationProfile, value);
        }
    }
     public async Task LoadProfileAsync(string uid, OrganizationProfileModel _organizationProfile)
    {   
            OrganizationProfile = await _myProfileService.UserProfileAsync(uid, _organizationProfile);
    }
}
