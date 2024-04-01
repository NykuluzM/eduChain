using System.Windows.Input;
using eduChain.Views.ContentPages;
using System.ComponentModel;
using eduChain.Models;

namespace eduChain.ViewModels{

    public class RegisterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public RegisterModel RegisterModel { get; set; } 
        public ICommand RegisterCommand { get; }
        public ICommand NavigateBackCommand { get; private set; }
       

        private static RegisterViewModel _instance;

        public static RegisterViewModel GetInstance()
        {
            {
                if (_instance == null)
                {
                    _instance = new RegisterViewModel();
                }
                return _instance;
            }
        }
        private string _email = string.Empty;
        public string Email
        {
                get { return _email; }
                set
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
        }
        private string _password = string.Empty;
        public string Password
        {
            get { return _password; }
            set
            {
                if(_password != value)
                {
                     _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }
        public RegisterViewModel(){
            this.RegisterModel = new RegisterModel();
        }
       protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler? handler = PropertyChanged;
            if (handler != null)
            {
                handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            else
            {
                throw new NullReferenceException("PropertyChanged event is not subscribed to.");
            }
        }
        public void Reset()
        {
            Email = string.Empty;
            Password = string.Empty;
            // Reset other properties as needed
        }

    }
}


