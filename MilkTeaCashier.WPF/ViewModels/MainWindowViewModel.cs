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
            var dialogService = new DialogHelper();
            var fileExportService = new FileExportService(dialogService, dialogService);
            var reportingService = new ReportingService(new UnitOfWork());
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
