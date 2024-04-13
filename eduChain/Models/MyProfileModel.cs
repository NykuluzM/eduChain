using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace eduChain.Models;

public class MyProfileModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    
    public string Email { get; set; }
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
    private string _createdAt;
    public string CreatedAt {
                                get { return _createdAt; }
                                set
                                {
                                    if(_createdAt != value){
                                        _createdAt = value;
                                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CreatedAt)));
                                    }
                                }
                            }
    public string FirebaseId { get; set; }
                                
     private byte[] _profilePic;
    [AllowNull]
    public byte[] ProfilePic {
                                get { return _profilePic; }
                                set
                                {
                                    if(_profilePic != value){
                                        _profilePic = value;
                                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProfilePic)));
                                    }
                                }
                            }

}
