using InputKit.Shared.Validations;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace eduChain.ViewModels{
public class ConfirmPasswordValidation : IValidation
{
    public string Message { get; set; } = "Passwords do not match";
    RegisterViewModel viewModel = RegisterViewModel.GetInstance(); 
    public bool Validate(object value)
    {
        string truePassword = viewModel.Password;
 
        if(value is string text){
            bool isValid = truePassword == text;
            return isValid;
        }
        return false;
    }

}
}
