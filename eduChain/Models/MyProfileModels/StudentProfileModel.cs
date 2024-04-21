using System.ComponentModel;

namespace eduChain;

public class StudentProfileModel : INotifyPropertyChanged, IProfileModel
{
	public event PropertyChangedEventHandler PropertyChanged;
     private static StudentProfileModel instance;
    private static readonly object lockObject = new object();

    
    // public MyProfileModel(){
        //LoadProfileImage();
    // }
    
        public static StudentProfileModel Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new StudentProfileModel();
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
    public string FirebaseId { get; set; }
    private string _firstName;
    public string FirstName { 
                                get { return _firstName; }
                                set{
                                    if(_firstName != value){
                                        _firstName = value;
                                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FirstName)));
                                        UpdateFullName();
                                    }
                                }
                            }
    private string _lastName;
    public string LastName  { 
                                get { return _lastName; }
                                set{
                                    if(_lastName != value){
                                        _lastName = value;
                                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LastName)));
                                        UpdateFullName();
                                    }
                                }
                            }
    private string _fullName;
    public string FullName { 
                                get { return _fullName; }
                                set{
                                    if(_fullName != value){
                                        _fullName = value;
                                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullName)));
                                    }
                                }
                            }
    private void UpdateFullName(){
        FullName = $"{FirstName} {LastName}";
    }
    public string Email{ get; set;}
    public string User_Firebase_Id { get; set; }
}