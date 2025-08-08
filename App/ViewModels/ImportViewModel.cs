using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ookii.Dialogs.Wpf;
using System.Collections.ObjectModel;
using System.IO;

namespace BankFlowAnalyzer.ViewModels
{
    public partial class ImportViewModel : ObservableObject
    {
        [ObservableProperty] private string? selectedFolder;
        [ObservableProperty] private string status = "就绪";
        public ObservableCollection<object> PreviewRows { get; } = new();

        public IRelayCommand BrowseFolderCommand { get; }
        public IRelayCommand ImportSampleCommand { get; }

        public ImportViewModel()
        {
            BrowseFolderCommand = new RelayCommand(OnBrowseFolder);
            ImportSampleCommand = new RelayCommand(OnImportSample);
        }

        private void OnBrowseFolder()
        {
            var dlg = new VistaFolderBrowserDialog();
            if (dlg.ShowDialog() == true)
            {
                SelectedFolder = dlg.SelectedPath;
                Status = "已选择文件夹：" + SelectedFolder;
            }
        }

        private void OnImportSample()
        {
            // 仅演示：加载示例数据到预览表格
            PreviewRows.Clear();
            PreviewRows.Add(new { 交易时间 = "2025-01-01 10:00:00", 金额 = 1000.00m, 收支 = "收入", 摘要 = "工资", 对方账号 = "****1234" });
            PreviewRows.Add(new { 交易时间 = "2025-01-03 15:30:00", 金额 = -200.00m, 收支 = "支出", 摘要 = "超市", 对方账号 = "****5678" });
            Status = "已导入示例数据（仅预览，不写库）";
        }
    }
}