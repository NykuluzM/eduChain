using System.ComponentModel;
using eduChain.Models;
using System.Windows.Input;
using eduChain;
using SkiaSharp;
using System.Collections.ObjectModel;

public class HomePageViewModel : ViewModelBase
{
    public ICommand LogoutCommand { get; }


    private bool _isLoading = true;
    public bool IsLoading
    {
        get { return _isLoading; }
        set
        {
            _isLoading = value;
            OnPropertyChanged(nameof(IsLoading));
        }
    }
    private static HomePageViewModel _instance;
    public static HomePageViewModel Instance
    {
        get
        {
            if (_instance == null)
            {
                return new HomePageViewModel();
            }
            return _instance;
            
        }
    }

    public async Task InitializeAsync()
    {
        AffiliatedOrganizations = await AffiliationsDatabaseService.Instance.GetAffiliatedOrganizationTo(UsersProfile.FirebaseId);
        AffiliatedStudents = await AffiliationsDatabaseService.Instance.GetAffiliatedStudentsTo(UsersProfile.FirebaseId);
        AffiliationRequestsStudents = await AffiliationsDatabaseService.Instance.GetAffiliationRequestsStudents(UsersProfile.FirebaseId);
        AffiliationRequestsOrganizations = await AffiliationsDatabaseService.Instance.GetAffiliationRequestsOrganizations(UsersProfile.FirebaseId);

    }
    public HomePageViewModel()
    {
        LogoutCommand = new Command(ExecuteLogout);
      
    }
       private async void ExecuteLogout()
    {
        // Handle the logout logic here
        Preferences.Default.Set("email", string.Empty);
        Preferences.Default.Set("password", string.Empty);
        Preferences.Default.Set("IsLoggedIn", false);
        Preferences.Default.Set("role", string.Empty);
        await Shell.Current.GoToAsync("//loginPage");
    }
    public async Task<bool> UIDVerify(string cid, string context)
    {
        if (context == "org")
        {
            return await AffiliationsDatabaseService.Instance.Exists(cid, "organization");

        }
        else
        {
            return await AffiliationsDatabaseService.Instance.Exists(cid, "student");
        }
    }
    public async Task SendAffRequest(string cid)
    {
        await AffiliationsDatabaseService.Instance.SendAffiliationRequest(UsersProfile.FirebaseId, cid);
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
}
