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
        await MyProfileService.Instance.UpdateStudentUserProfileAsync(StudentProfile);

    }
    public async Task<bool> UIDVerify(string cid, string context)
    {
        if(context == "org")
        {
            return await AffiliationsDatabaseService.Instance.Exists(cid, "organization");

        } else
        {
            return await AffiliationsDatabaseService.Instance.Exists(cid, "student");
        }
    }
    public async Task SendAffRequest(string cid)
    {
        await AffiliationsDatabaseService.Instance.SendAffiliationRequest(StudentProfile.FirebaseId, cid);
    }
    public async Task AcceptAffiliations(ObservableCollection<object> affiliations)
    {
        var affiliationsModelCollection = new ObservableCollection<AffiliationsModel>(affiliations.Cast<AffiliationsModel>());

        await AffiliationsDatabaseService.Instance.AcceptAffiliationRequest(affiliationsModelCollection, UsersProfile.FirebaseId);
    }
    public async Task RejectAffiliations(ObservableCollection<object> affiliations)
    {
        var affiliationsModelCollection = new ObservableCollection<AffiliationsModel>(affiliations.Cast<AffiliationsModel>());

        await AffiliationsDatabaseService.Instance.RejectAffiliationRequest(affiliationsModelCollection, UsersProfile.FirebaseId);
    }
    public async Task RemoveAffiliations(ObservableCollection<object> affiliations)
    {
        var affiliationsModelCollection = new ObservableCollection<AffiliationsModel>(affiliations.Cast<AffiliationsModel>());

        await AffiliationsDatabaseService.Instance.RemoveAffiliations(affiliationsModelCollection, UsersProfile.FirebaseId);
    }
    private ObservableCollection<AffiliationsModel> _affiliatedorganizations;
    public ObservableCollection<AffiliationsModel> AffiliatedOrganizations
    {
        get { return _affiliatedorganizations; }
        set
        {
            _affiliatedorganizations = value;
            OnPropertyChanged(nameof(AffiliatedOrganizations));
        }
    }

    private ObservableCollection<AffiliationsModel> _affiliatedstudents;
    public ObservableCollection<AffiliationsModel> AffiliatedStudents
    {
        get { return _affiliatedstudents; }
        set
        {
            _affiliatedstudents = value;
            OnPropertyChanged(nameof(AffiliatedStudents));
        }
    }
    private ObservableCollection<AffiliationsModel> _affiliationrequestsstudents;
    public ObservableCollection<AffiliationsModel> AffiliationRequestsStudents
    {
        get { return _affiliationrequestsstudents; }
        set
        {
            _affiliationrequestsstudents = value;
            OnPropertyChanged(nameof(AffiliationRequestsStudents));
        }
    }
    private ObservableCollection<AffiliationsModel> _affiliationrequestsorganizations;
    public ObservableCollection<AffiliationsModel> AffiliationRequestsOrganizations
    {
        get { return _affiliationrequestsorganizations; }
        set
        {
            _affiliationrequestsorganizations = value;
            OnPropertyChanged(nameof(AffiliationRequestsOrganizations));
        }
    }

    public async void InitializeAsync()
    {
        AffiliatedOrganizations = await AffiliationsDatabaseService.Instance.GetAffiliatedOrganizationTo(UsersProfile.FirebaseId);
        AffiliatedStudents = await AffiliationsDatabaseService.Instance.GetAffiliatedStudentsTo(UsersProfile.FirebaseId);
        AffiliationRequestsStudents = await AffiliationsDatabaseService.Instance.GetAffiliationRequestsStudents(UsersProfile.FirebaseId);
        AffiliationRequestsOrganizations = await AffiliationsDatabaseService.Instance.GetAffiliationRequestsOrganizations(UsersProfile.FirebaseId);

    }

}
