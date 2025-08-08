using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BankFlowAnalyzer.Services;
using Microsoft.Extensions.DependencyInjection;

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
            _ = OnSearchAsync();
        }

        private async Task OnSearchAsync()
        {
            var sp = BankFlowAnalyzer.App.HostContainer!.Services;
            var svc = sp.GetRequiredService<IAnalysisService>();
            await svc.SeedIfEmptyAsync();
            var rows = await svc.PagedAsync(1, 200, Keyword);
            Items.Clear();
            foreach (var r in rows) Items.Add(r);
        }
    }
}