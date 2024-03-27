using Syncfusion.Maui.DataForm;	
using eduChain.ViewModel;
namespace eduChain.View
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
			var viewModel = new DataFormViewModel();
            BindingContext = viewModel;

            // Initialize the DataFormItemManagerEditorExt
            var dataFormItemManager = new DataFormItemManagerEditorExt();

            // Set the ItemManager of the SfDataForm
            loginForm.ItemManager = dataFormItemManager;

		}
	}
	
	public class DataFormItemManagerEditorExt : DataFormItemManager
	{
    public override void InitializeDataLabel(DataFormItem dataFormItem, Label label)
    {
        label.Background = Colors.Orange;
        label.VerticalOptions = LayoutOptions.Center;
        label.CharacterSpacing = 2;
        label.Padding = new Thickness(5, 0, 5, 0);
        label.Margin = new Thickness(0, 0, 5, 0);
        label.FontSize = 18;
        FormattedString formattedString = new FormattedString();
        formattedString.Spans.Add(new Span { Text = label.Text, TextColor = Colors.White});
        formattedString.Spans.Add(new Span { Text = " *", TextColor = Colors.Red});
        label.FormattedText = formattedString;
    }
}
}