namespace eduChain.Models.MyProfileModels;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using SkiaSharp;
public class OrganizationProfileModel : INotifyPropertyChanged, IProfileModel
{
      public event PropertyChangedEventHandler PropertyChanged;
    private static OrganizationProfileModel instance;
    private static readonly object lockObject = new object();

    
    // public MyProfileModel(){
        //LoadProfileImage();
    // }
    
        public static OrganizationProfileModel Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new OrganizationProfileModel();
                        }
                    }
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }

    private string _email;
    public string Email {
                            get { return _email; }
                            set
                            {
                                if(_email != value){
                                    _email = value;
                                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Email)));
                                }
                            }
                        }
    public string User_Firebase_Id { get; set; }

    private string _name;
    public string Name {
                                get { return _name; }
                                set
                                {
                                    if(_name != value){
                                        _name = value;
                                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
                                    }
                                }
                            }
                    
    private string _orgName;
    public string OrgName{
                            get { return _orgName; }
                            set
                            {
                                if(_orgName != value){
                                    _orgName = value;
                                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OrgName)));
                                }
                            }
                         }
    private string _type;
    public string Type {
                            get { return _type; }
                            set
                            {
                                if(_type != value){
                                    _type = value;
                                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Type)));
                                }
                            }
                        }
    public string user_firebase_id { get; set; }    

}