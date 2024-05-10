using eduChain.ViewModels;
using eduChain.Models;
using UraniumUI;
using CommunityToolkit.Maui.Views;
namespace eduChain.Views.ContentPages
{
    public partial class RegisterStudPage : ContentPage
    {
        public RegisterStudPage()
        {
            InitializeComponent();

            
            BindingContext = RegisterStudViewModel.GetInstance();
			

        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            BindingContext = null;	
            RegisterStudViewModel.ResetInstance();
        }
    }
}
