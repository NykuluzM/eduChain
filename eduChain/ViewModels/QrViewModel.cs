
using System.Collections.ObjectModel;
using System.Windows.Input;
using eduChain.Models;
namespace eduChain.ViewModels
{
    public class QrViewModel : ViewModelBase
    {
        public ICommand DeleteCommand { get; }

        private ObservableCollection<QrModel> _qrList;
        public ObservableCollection<QrModel> QrList
        {
            get { return _qrList; }
            set
            {
                _qrList = value;
                OnPropertyChanged(nameof(QrList));
            }
        }
        public QrViewModel()
        {
            DeleteCommand = new Command(async () => await Remove());
        }
        public async Task<bool> InitializeDataAsync()
        {
            try
            {
                var success = await QrDatabaseService.Instance.RemoveExpiredQr();
                if (!success)
                {
                    return false;
                }
                QrList = await QrDatabaseService.Instance.GetMyQrList(UsersProfile.FirebaseId);
                if(QrList == null && QrList.Count < 1)
                {
                    return false;
                } else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                Console.Error.WriteLine($"Error in InitializeDataAsync: {ex.Message}");
                return false;
            }
        }
        private async Task Remove()
        {
            var selectedItems = QrList.Where(org => org.IsSelected)
                                  .ToList();
            if(selectedItems.Count == 0)
            {
                await Shell.Current.DisplayAlert("Error", "Please select an item", "OK");
                return;
            }
            var selectedItemsObservable = new ObservableCollection<QrModel>(selectedItems);

            var success = await QrDatabaseService.Instance.RemoveQr(selectedItemsObservable);
            if(success)
            {
                foreach (var item in selectedItemsObservable)
                {
                    QrList.Remove(item);
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Failed to remove QR, Try Again Later", "OK");
                return;
            }
            await Shell.Current.DisplayAlert("Success", "Selected QR/s Removed", "OK");
            return;
        }
    }
    
}
