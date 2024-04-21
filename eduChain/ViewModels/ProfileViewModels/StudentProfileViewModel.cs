﻿using eduChain.Services;
using eduChain.ViewModels.ProfileViewModels;

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
            StudentProfile = await _myProfileService.StudentUserProfileAsync(uid, _studentProfile);
    }
    public async Task UpdateProfileAsync(){
            await MyProfileService.Instance.UpdateStudentUserProfileAsync(StudentProfile);
            await UploadImageToSupabase(imageBytes,Preferences.Default.Get("firebase_uid", String.Empty));
    } 
}