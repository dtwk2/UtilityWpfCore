using System.Windows;

namespace Suggest.WPF.FileSystem
{
    public class SuggestPathBox : Suggest.WPF.SuggestBox
    {
        static SuggestPathBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SuggestPathBox), new FrameworkPropertyMetadata(typeof(SuggestPathBox)));
        }

        public SuggestPathBox()
        {
            //this.Validation = new InvalidValidationRule();
        }
    }
}