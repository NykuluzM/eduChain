
using System.Collections.ObjectModel;
using eduChain.Models;
namespace eduChain.ViewModels
{
    public class QrViewModel : ViewModelBase
    {
        private ObservableCollection<QrModel> _qrList;
        public ObservableCollection<QrModel> QrList
        {
            get => _qrList;
            set
            {
                _qrList = value;
                OnPropertyChanged(nameof(QrList));
            }
        }

    }
}
