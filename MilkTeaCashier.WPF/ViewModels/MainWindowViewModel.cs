using MilkTeaCashier.Data.UnitOfWork;
using MilkTeaCashier.Service.Services;
using MilkTeaCashier.WPF.Utilities;
using MilkTeaCashier.WPF.Views;
using MvvmHelpers;
using System.Windows;
using System.Windows.Input;


namespace MilkTeaCashier.WPF.ViewModels
{

    /// <summary>
    /// This class is Temporary, Updated for opening ReportView to test out 
    /// Sale Reporting Features
    /// </summary>
    public class MainWindowViewModel : BaseViewModel
    {
        public ICommand OpenReportViewCommand { get; }

        public MainWindowViewModel()
        {
            OpenReportViewCommand = new RelayCommand
            (
                execute: param => OpenReportView(param),
                canExecute: param => CanOpenSalesReport(),
                onException: ex => HandleError(ex)
            );
        }
        private void OpenReportView(object parameter)
        {
            var reportingService = new ReportingService(new UnitOfWork()); // Replace with DI if available
            var fileExportService = new FileExportService();

            var reportViewModel = new ReportViewModel(reportingService, fileExportService);

            var reportWindow = new ReportView
            {
                DataContext = reportViewModel,
                Width = 1024,
                Height = 768,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            reportWindow.ShowDialog();
        }

        private async Task OpenSalesReportAsync()
        {
            try
            {
                await Task.Delay(100);

                var reportingService = new ReportingService(new UnitOfWork()); // Replace with actual DI if available
                var fileExportService = new FileExportService();

                var reportViewModel = new ReportViewModel(reportingService, fileExportService);

                var reportWindow = new ReportView
                {
                    DataContext = reportViewModel,
                    Width = 1024,
                    Height = 768,
                    WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen
                };
                reportWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private bool CanOpenSalesReport()
        {
            // Example condition: return false if some prerequisite is missing
            return true; //tmp
        }

        private void HandleError(Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }


}
