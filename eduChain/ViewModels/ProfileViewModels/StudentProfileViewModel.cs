using eduChain.Models;
using eduChain.Services;
using eduChain.ViewModels.ProfileViewModels;
using System.Collections.ObjectModel;

namespace eduChain;

public class StudentProfileViewModel : BaseProfileViewModel
{
    private StudentProfileModel _studentProfile;

    public StudentProfileModel StudentProfile
    {
        get { return _studentProfile; }
        set
        {
        SetProperty(ref _studentProfile, value);
        }
    }
    public async Task LoadProfileAsync(string uid, StudentProfileModel _studentProfile)
    {   
            StudentProfile = await _myProfileService.UserProfileAsync(uid, _studentProfile);
    }
    public async Task UpdateProfilePicture()
    {
       await UploadImageToSupabase(imageBytes, Preferences.Default.Get("firebase_uid", String.Empty));
    }
    public async Task UpdateProfileAsync(){
        await MyProfileService.Instance.UpdateStudentUserProfileAsync(StudentProfile, UsersProfile.DisplayName);

    }
   

}
