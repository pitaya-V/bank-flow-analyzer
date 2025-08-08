using System.Windows;

namespace BankFlowAnalyzer.Views
{
    public partial class ProjectPickerWindow : Window
    {
        public ProjectPickerWindow()
        {
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DataContext is BankFlowAnalyzer.ViewModels.ProjectPickerViewModel vm)
            {
                _ = vm.OpenSelectedAsync(); // fire-and-forget
            }
        }
    }
}