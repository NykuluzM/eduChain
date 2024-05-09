

using System.ComponentModel;

namespace eduChain.Models
{
   public class AffiliationsModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsSelected{ get; set;}

        public string Id { get; set; }
        public string Name { get; set; }
        public string DateEstablished { get; set; }
    }
}
