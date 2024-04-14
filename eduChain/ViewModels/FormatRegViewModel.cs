using System.Collections.ObjectModel;
namespace eduChain.ViewModels;

public class FormatRegViewModel : ViewModelBase
{

        public FormatRegViewModel()
        {
            Items = new ObservableCollection<string>
            {
                "Student",
                "Organization",
                "Guardian"
            };
        }

        private ObservableCollection<string> _items;
        public ObservableCollection<string> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }
}
