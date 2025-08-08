using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BankFlowAnalyzer.ViewModels
{
    public partial class TransactionsViewModel : ObservableObject
    {
        [ObservableProperty] private string? keyword;
        public ObservableCollection<object> Items { get; } = new();

        public IRelayCommand SearchCommand { get; }

        public TransactionsViewModel()
        {
            SearchCommand = new AsyncRelayCommand(OnSearchAsync);
            // 示例：初始两行
            Items.Add(new { tx_time = "2025-01-01 10:00:00", direction = "收入", amount = 1000, summary = "工资" });
            Items.Add(new { tx_time = "2025-01-03 15:30:00", direction = "支出", amount = 200, summary = "超市" });
        }

        private async Task OnSearchAsync()
        {
            await Task.Delay(50);
        }
    }
}