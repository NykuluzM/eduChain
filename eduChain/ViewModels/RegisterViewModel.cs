using System.Windows.Input;
using eduChain.Views.ContentPages;
using System.ComponentModel;
using eduChain.Models;

namespace eduChain.ViewModels{

    public class RegisterViewModel : INotifyPropertyChanged
    {
        public RegisterModel RegisterModel { get; set; } 
        public ICommand RegisterCommand { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void Register()
        {
            Shell.Current.GoToAsync("//registerPage");
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

        public RegisterViewModel(){
            this.RegisterModel = new RegisterModel();
            this.RegisterCommand = new Command(Register);

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
    }
}


