using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using BankFlowAnalyzer.Services;
using System.Threading.Tasks;
using System.IO;

namespace BankFlowAnalyzer.ViewModels
{
    public partial class ProjectPickerViewModel : ObservableObject
    {
        private readonly IProjectService _project;

        [ObservableProperty] private string? newCaseNo;
        [ObservableProperty] private string? newTitle;
        [ObservableProperty] private string status = "请选择或创建项目";
        [ObservableProperty] private ProjectInfo? selectedRecent;
        [ObservableProperty] private bool deletePhysical;

        public ObservableCollection<ProjectInfo> Recents { get; } = new();

        public IRelayCommand CreateCommand { get; }
        public IRelayCommand OpenSelectedCommand { get; }
        public IRelayCommand RemoveSelectedCommand { get; }
        public IRelayCommand OpenFromFileCommand { get; }
        public IRelayCommand CancelCommand { get; }

        public ProjectPickerViewModel(IProjectService project)
        {
            _project = project;
            CreateCommand = new AsyncRelayCommand(CreateAsync);
            OpenSelectedCommand = new AsyncRelayCommand(OpenSelectedAsync);
            RemoveSelectedCommand = new AsyncRelayCommand(RemoveSelectedAsync);
            OpenFromFileCommand = new AsyncRelayCommand(OpenFromFileAsync);
            CancelCommand = new RelayCommand(() => CloseWindow(false));
            _ = LoadRecentsAsync();
        }

        private async Task LoadRecentsAsync()
        {
            Recents.Clear();
            foreach (var r in await _project.GetRecentAsync(20)) Recents.Add(r);
        }

        private async Task CreateAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewCaseNo) || string.IsNullOrWhiteSpace(NewTitle))
                {
                    Status = "请输入案号与标题";
                    return;
                }
                await _project.CreateAsync(NewCaseNo!.Trim(), NewTitle!.Trim());
                Status = "创建成功";
                CloseWindow(true);
            }
            catch (Exception ex) { Status = ex.Message; }
        }

        public async Task OpenSelectedAsync()
        {
            if (SelectedRecent is null) { Status = "请先选择一个最近项目"; return; }
            try
            {
                await _project.OpenAsync(SelectedRecent.DbPath);
                CloseWindow(true);
            }
            catch (Exception ex) { Status = ex.Message; }
        }

        private async Task OpenFromFileAsync()
        {
            var dlg = new OpenFileDialog { Filter = "SQLite 数据库 (*.db)|*.db|所有文件 (*.*)|*.*" };
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    await _project.OpenAsync(dlg.FileName);
                    CloseWindow(true);
                }
                catch (Exception ex) { Status = ex.Message; }
            }
        }

        private async Task RemoveSelectedAsync()
        {
            if (SelectedRecent is null) return;
            try
            {
                // 先从最近列表移除
                await _project.RemoveRecentAsync(SelectedRecent.DbPath);

                // 可选：物理删除 .db 和同目录
                if (DeletePhysical)
                {
                    try
                    {
                        if (File.Exists(SelectedRecent.DbPath))
                        {
                            var dir = Path.GetDirectoryName(SelectedRecent.DbPath)!;
                            File.Delete(SelectedRecent.DbPath);
                            // 如果目录是我们标准目录且已空，尝试删除目录
                            if (Directory.Exists(dir) && Directory.GetFileSystemEntries(dir).Length == 0)
                                Directory.Delete(dir);
                        }
                    }
                    catch (Exception ex) { Status = "删除文件失败：" + ex.Message; }
                }

                await LoadRecentsAsync();
            }
            catch (Exception ex) { Status = ex.Message; }
        }

        private void CloseWindow(bool dialogResult)
        {
            // 可靠关闭：找到第一个 ProjectPickerWindow 并设置 DialogResult
            foreach (System.Windows.Window w in System.Windows.Application.Current.Windows)
            {
                if (w is BankFlowAnalyzer.Views.ProjectPickerWindow)
                {
                    w.Dispatcher.Invoke(() => {
                        w.DialogResult = dialogResult;
                        w.Close();
                    });
                    break;
                }
            }
        }
    }
}
