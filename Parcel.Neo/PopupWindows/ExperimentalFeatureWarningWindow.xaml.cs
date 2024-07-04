using System.Windows;

namespace Parcel.Neo.PopupWindows
{
    public partial class ExperimentalFeatureWarningWindow : Window
    {
        #region Construction
        public ExperimentalFeatureWarningWindow(string message)
        {
            InitializeComponent();

            ContentTextBlock.Text = message;
        }
        #endregion

        #region Events
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        #endregion
    }
}
