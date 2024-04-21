using eduChain.ViewModels;
using eduChain.Models;
using UraniumUI;
using CommunityToolkit.Maui.Views;
namespace eduChain.Views.ContentPages
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();

            
            BindingContext = RegisterViewModel.GetInstance();
			

        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            BindingContext = null;	
            RegisterViewModel.ResetInstance();
        }
    }
}
