using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;

namespace BankFlowAnalyzer.ViewModels
{
    public partial class AnalysisViewModel : ObservableObject
    {
        public IEnumerable<ISeries> Series { get; }
        public Axis[] XAxes { get; }
        public Axis[] YAxes { get; }

        public AnalysisViewModel()
        {
            // 示例月度净额
            var values = new double[] { 12000, 8000, 9500, 11000, 7000, 13000 };
            Series = new ISeries[] { new ColumnSeries<double> { Values = values, Name = "净流入" } };
            XAxes = new[] { new Axis { Labels = new[] { "1月","2月","3月","4月","5月","6月" } } };
            YAxes = new[] { new Axis { } };
        }
    }
}