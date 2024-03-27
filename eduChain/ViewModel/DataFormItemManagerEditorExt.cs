using Syncfusion.Maui.DataForm;
using Syncfusion.Maui.Graphics;
namespace eduChain.ViewModel
{
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
        formattedString.Spans.Add(new Span { Text = label.Text, TextColor = Colors.White });
        formattedString.Spans.Add(new Span { Text = " *", TextColor = Colors.Red });
        label.FormattedText = formattedString;
        }
    }
}