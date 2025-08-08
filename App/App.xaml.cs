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
                    services.AddSingleton<BankFlowAnalyzer.ViewModels.ProjectPickerViewModel>();

                    // Services
                    services.AddSingleton<BankFlowAnalyzer.Services.IProjectService, BankFlowAnalyzer.Services.ProjectService>();
                    services.AddSingleton<BankFlowAnalyzer.Services.IAnalysisService, BankFlowAnalyzer.Services.AnalysisService>();
                })
                .Build();

            base.OnStartup(e);

            try
            {
                // 🔴 1) 关键：在显示选择窗口之前，禁止“最后窗口关闭就退出”
                ShutdownMode = ShutdownMode.OnExplicitShutdown;

                var pickerVm = HostContainer.Services.GetRequiredService<BankFlowAnalyzer.ViewModels.ProjectPickerViewModel>();
                var picker = new BankFlowAnalyzer.Views.ProjectPickerWindow { DataContext = pickerVm };

                picker.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("打开项目选择窗口失败: " + ex.Message);
                Shutdown();
                return;
            }

            var projSvc = HostContainer.Services.GetRequiredService<BankFlowAnalyzer.Services.IProjectService>();
            if (projSvc.Current is null)
            {
                Shutdown();
                return;
            }

            // 2) 创建并注册主窗口
            var win = new BankFlowAnalyzer.Views.MainWindow
            {
                Title = $"银行流水分析系统 - {projSvc.Current.Title}"
            };

            // 🔴 3) 告诉应用：这个是“主窗口”
            MainWindow = win;

            // 🔴 4) 现在再恢复正常关停策略——主窗口关闭才退出
            ShutdownMode = ShutdownMode.OnMainWindowClose;

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