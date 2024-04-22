using System.ComponentModel;

namespace eduChain;

public class GuardianProfileModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public string Email { get; set; }
    public string FirebaseId { get; set; }
}
