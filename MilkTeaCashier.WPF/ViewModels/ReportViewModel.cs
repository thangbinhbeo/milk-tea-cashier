using Microsoft.Office.Interop.Excel;
using MilkTeaCashier.Data.DTOs;
using MilkTeaCashier.Service.Interfaces;
using MilkTeaCashier.WPF.Utilities;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MilkTeaCashier.WPF.ViewModels
{
    public class ReportViewModel : BaseViewModel
    {
        #region ___FIELDS AND PROPERTIES___
        public const string SALE_REPORT_CSV_FILE_NAME = "SaleReport.csv";
        public const string SALE_REPORT_PDF_FILE_NAME = "SaleReport.pdf";
        private readonly IReportingService _reportingService;
        private readonly IFileExportService _fileExportService;

        public ObservableRangeCollection<RevenueReportDto> RevenueReports { get; set; }
        public ObservableRangeCollection<TopSellingProductDto> TopSellingProducts { get; set; }
        public List<TopSellingProductDto> AllTopSellingProducts { get; set; } // Original unfiltered list


        public DateTime SelectedStartDate { get; set; } = DateTime.Now.AddDays(-7); // Default 7 days report
        public DateTime SelectedEndDate { get; set; } = DateTime.Now;

        public double? MinRevenue { get; set; }
        public double? MaxRevenue { get; set; }
        public int? TopN { get; set; }

        public RelayCommand LoadReportsCommand { get; }
        public RelayCommand ExportToCSVCommand { get; }
        public RelayCommand ExportToPDFCommand { get; }
        public RelayCommand ApplyFiltersCommand { get; }

        private bool _isOperationInProgress;

        public bool IsOperationInProgress
        {
            get => _isOperationInProgress;
            set => SetProperty(ref _isOperationInProgress, value); // Use INotifyPropertyChanged or equivalent
        }

        public ReportViewModel(IReportingService reportingService, IFileExportService fileExportService)
        {
            _reportingService = reportingService;
            _fileExportService = fileExportService;

            RevenueReports = new ObservableRangeCollection<RevenueReportDto>();
            TopSellingProducts = new ObservableRangeCollection<TopSellingProductDto>();

            LoadReportsCommand = new RelayCommand(
                async _ => await ExecuteWithState(LoadReportsAsync),
                _ => !IsOperationInProgress && CanLoadReports(),
                ex => HanleException("Failed to load report!", ex)
            );

            ExportToCSVCommand = new RelayCommand(
                async _ => await ExecuteWithState(() => ExportToCSVAsync()),
                _ => !IsOperationInProgress && CanExport(),
                ex => HanleException("Failed to export to CSV!", ex)
            );

            ExportToPDFCommand = new RelayCommand(
                async _ => await ExecuteWithState(() => ExportToPDFAsync()),
                _ => !IsOperationInProgress && CanExport(),
                ex => HanleException("Failed to export to PDF!", ex)
            );

            ApplyFiltersCommand = new RelayCommand(
                _ => ApplyFilters(),
                _ => !IsOperationInProgress
            );
        }
        #endregion

        #region ___MAIN METHODS___
        private async Task ExecuteWithState(Func<Task> asyncMethod)
        {
            IsOperationInProgress = true;
            try
            {
                await asyncMethod();
            }
            finally
            {
                IsOperationInProgress = false;
            }
        }

        private async Task ExportToCSVAsync()
        {
            var data = _reportingService.PrepareExportData(RevenueReports, TopSellingProducts);
            await _fileExportService.ExportToCSVAsync(data, SALE_REPORT_CSV_FILE_NAME);
        }

        private async Task ExportToPDFAsync()
        {
            var data = _reportingService.PrepareExportData(RevenueReports, TopSellingProducts);
            await _fileExportService.ExportToPDFAsync(data, SALE_REPORT_PDF_FILE_NAME);
        }

        private async Task LoadReportsAsync()
        {
            try
            {
                IsOperationInProgress = true;

                var revenueReports = await _reportingService.GetRevenueReportAsync(SelectedStartDate, SelectedEndDate);
                var topSellingProducts = await _reportingService.GetTopSellingProductsAsync(SelectedStartDate, SelectedEndDate);

                // Clear collections only after fetching data successfully
                RevenueReports?.Clear();
                TopSellingProducts?.Clear();
                AllTopSellingProducts?.Clear();
               

                AddRevenueReports(revenueReports);
                AddTopSellingProducts(topSellingProducts);
                // Store the original list for filtering
                AllTopSellingProducts = topSellingProducts.ToList();

                Console.WriteLine("Reports loaded successfully.");
            }
            catch (Exception ex)
            {
                HanleException("Failed to load reports", ex);
            }
            finally
            {
                IsOperationInProgress = false;
            }
        }


        private void ApplyFilters()
        {
            try
            {
                Console.WriteLine($"Applying Filters:");
                Console.WriteLine($"- Min Revenue: {MinRevenue}");
                Console.WriteLine($"- Max Revenue: {MaxRevenue}");
                Console.WriteLine($"- Top N: {TopN}");

                // Filter Revenue Reports
                if (RevenueReports.Any())
                {
                    var filteredReports = _reportingService.FilterRevenueReports(
                        startDate: SelectedStartDate,
                        endDate: SelectedEndDate,
                        minRevenue: MinRevenue,
                        maxRevenue: MaxRevenue
                    );

                    Console.WriteLine($"Filtered Reports Count: {filteredReports.Count}");
                    RevenueReports.ReplaceRange(filteredReports);
                }

                // Filter Top-Selling Products
                if (AllTopSellingProducts != null && AllTopSellingProducts.Any())
                {
                    var filteredTopProducts = AllTopSellingProducts // Use the original collection
                        .OrderByDescending(p => p.Revenue)
                        .Take(TopN ?? AllTopSellingProducts.Count) // Default to all if TopN is null
                        .ToList();

                    Console.WriteLine($"Filtered Top-Selling Products Count: {filteredTopProducts.Count}");
                    TopSellingProducts.ReplaceRange(filteredTopProducts);
                }
            }
            catch (Exception ex)
            {
                HanleException("Failed to apply filters", ex);
            }
        }

        private void HanleException(string message, Exception ex)
        {
            MessageBox.Show($"{message}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion

        #region ___HELPER METHODS___
        public bool CanLoadReports()
        {
            return SelectedStartDate <= SelectedEndDate; // Simple condition
        }

        public bool CanExport()
        {
            return RevenueReports != null && (RevenueReports.Any() || TopSellingProducts.Any());
        }

        public void AddRevenueReports(List<RevenueReportDto> revenueReports)
        {
            if (revenueReports == null) return;
            foreach (var report in revenueReports)
            {
                if (report == null || RevenueReports.Contains(report))
                    continue;
                RevenueReports.Add(report);
            }
        }

        public void AddTopSellingProducts(List<TopSellingProductDto> topSellingReports)
        {
            if (topSellingReports == null) return;
            foreach (var prd in topSellingReports)
            {
                if (prd == null || TopSellingProducts.Contains(prd))
                    continue;
                TopSellingProducts.Add(prd);
            }
        }
        #endregion
    }

}
