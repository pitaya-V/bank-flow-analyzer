using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using BankFlowAnalyzer.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace BankFlowAnalyzer.ViewModels
{
    public partial class AnalysisViewModel : ObservableObject
    {
        public IEnumerable<ISeries> Series { get; private set; }
        public Axis[] XAxes { get; private set; }
        public Axis[] YAxes { get; private set; }

        public AnalysisViewModel()
        {
            Series = new ISeries[] { new ColumnSeries<double> { Values = new double[] { } } };
            XAxes = new[] { new Axis { Labels = new string[] { } } };
            YAxes = new[] { new Axis { } };
            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            var sp = BankFlowAnalyzer.App.HostContainer!.Services;
            var svc = sp.GetRequiredService<IAnalysisService>();
            await svc.SeedIfEmptyAsync();
            var rows = await svc.MonthlyNetAsync();
            var labels = new List<string>();
            var values = new List<double>();
            foreach (var t in rows) { labels.Add(t.ym); values.Add(t.net); }
            Series = new ISeries[] { new ColumnSeries<double> { Values = values.ToArray(), Name = "净流入" } };
            XAxes = new[] { new Axis { Labels = labels.ToArray() } };
            OnPropertyChanged(nameof(Series));
            OnPropertyChanged(nameof(XAxes));
            OnPropertyChanged(nameof(YAxes));
        }
    }
}