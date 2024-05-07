using Windows.UI.Xaml.Controls;

namespace TestPaint
{
    public sealed partial class RenameCanvasDialogContent : ContentDialog
    {
        public string NewName { get; set; }

        public RenameCanvasDialogContent(string oldName = null)
        {
            this.InitializeComponent();
            textBox.Text = oldName ?? "";
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            NewName = textBox.Text;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
