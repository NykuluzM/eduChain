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
                                    }
                                }
                            }

    public string Email{ get; set;}
     private string _gender;
    public string Gender   {
                                get { return _gender; }
                                set
                                {
                                    if(_gender != value){
                                        _gender = value;
                                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Gender)));
                                    }
                                }
                            }
    private string birth_date;
    public string BirthDate {
                                get { return birth_date; }
                                set
                                {
                                    if(birth_date != value){
                                        birth_date = value;
                                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BirthDate)));
                                          // Calculate age based on birth date
                                        if (DateTime.TryParse(value, out DateTime birthDateTime))
                                        {
                                            TimeSpan ageSpan = DateTime.Now - birthDateTime;
                                            int age = (int)(ageSpan.Days / 365.25); // Approximate age in years
                                            Age = age.ToString(); // Update the Age property with the calculated age
                                        }
                                        else
                                        {
                                            // Handle invalid birth date format
                                            // For example, set Age to null or an empty string
                                            Age = null; // or Age = "";
                                        }
                                    }
                                }
                            }
private string age;
public string Age {
    get { return age; }
    set
    {
        if (age != value)
        {
            age = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Age)));
        }
    }
}
    public string User_Firebase_Id { get; set; }
}