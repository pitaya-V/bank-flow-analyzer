using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Windows;

namespace BankFlowAnalyzer
{
    public partial class App : System.Windows.Application
    {
        public static IHost? HostContainer { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            HostContainer = Host.CreateDefaultBuilder(e.Args)
                .ConfigureServices((ctx, services) =>
                {
                    // ViewModels
                    services.AddSingleton<BankFlowAnalyzer.ViewModels.MainWindowViewModel>();
                    services.AddSingleton<BankFlowAnalyzer.ViewModels.ImportViewModel>();
                    services.AddSingleton<BankFlowAnalyzer.ViewModels.TransactionsViewModel>();
                    services.AddSingleton<BankFlowAnalyzer.ViewModels.AnalysisViewModel>();
                })
                .Build();

            base.OnStartup(e);

            var win = new Views.MainWindow();
            win.Show();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (HostContainer is not null)
                await HostContainer.StopAsync(TimeSpan.FromSeconds(2));
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}