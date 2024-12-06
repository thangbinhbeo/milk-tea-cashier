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

        public DateTime SelectedStartDate { get; set; } = DateTime.Now.AddDays(-7); //default 7 days report
        public DateTime SelectedEndDate   { get; set; } = DateTime.Now;

        public RelayCommand LoadReportsCommand { get; }
        public RelayCommand ExportToCSVCommand { get; }
        public RelayCommand ExportToPDFCommand { get; }

        public ReportViewModel(IReportingService reportingService, IFileExportService fileExportService)
        {
            _reportingService = reportingService;
            _fileExportService = fileExportService;

            RevenueReports = new ObservableRangeCollection<RevenueReportDto>();
            TopSellingProducts = new ObservableRangeCollection<TopSellingProductDto>();

            LoadReportsCommand = new RelayCommand
            (
                async _ => LoadReportsAsync(),
                _ => CanLoadReports(),
                ex => HanleException("Failed to load Report!!!", ex)
            );

            ExportToCSVCommand = new RelayCommand
            (
                async _=> await ExportToCSVAsync(),
                _ => CanExport(),
                 ex => HanleException("Failed to Export Report to csv!!!", ex)
            );
            ExportToPDFCommand = new RelayCommand
            (
                async _ => await ExportToPDFAsync(),
                _ => CanExport(),
                 ex => HanleException("Failed to Export Report to csv!!!", ex)
            );
        }

        #endregion

        #region ___MAIN METHODS___
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

        private async void LoadReportsAsync()
        {
            RevenueReports ??= new();
            RevenueReports.Clear();

            TopSellingProducts ??= new();
            TopSellingProducts.Clear();

            var revenueReports = await _reportingService.GetRevenueReportAsync(SelectedStartDate, SelectedEndDate);
            var topSellingProducts = await _reportingService.GetTopSellingProductsAsync(SelectedStartDate, SelectedEndDate);
            AddRevenueReports(revenueReports);
            AddTopSellingProducts(topSellingProducts);
        }

        
        private void HanleException(string message, Exception ex)
        {
            MessageBox.Show($"{message}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion

        #region ___HELPER METHODS___
        public bool CanLoadReports()
        {
            return SelectedStartDate <= SelectedEndDate; //Simple condition
        }
        public bool CanExport()
        {
            return RevenueReports == null ? false : (RevenueReports.Any() || TopSellingProducts.Any());
        }
        public void AddRevenueReports(List<RevenueReportDto> revenueReports)
        {
            if (revenueReports == null) return;
            foreach (RevenueReportDto report in revenueReports)
            {
                if (report == null || RevenueReports.Contains(report)) 
                    continue;
                RevenueReports.Add(report);
            }
        }
        public void AddRevenueReport(RevenueReportDto revenueReport)
        {
            if (revenueReport == null) return;
            if (!RevenueReports.Contains(revenueReport))
            {
                RevenueReports.Add(revenueReport);
            }
        }
        public void AddTopSellingProducts(List<TopSellingProductDto> topSellingReports)
        {
            if (topSellingReports == null) return;
            foreach (TopSellingProductDto prd in topSellingReports)
            {
                if (prd == null || TopSellingProducts.Contains(prd))
                    continue;
                TopSellingProducts.Add(prd);
            }
        }
        public void AddTopSellingProduct(TopSellingProductDto topSellingProduct)
        {
            if (topSellingProduct == null) return;
            if (!TopSellingProducts.Contains(topSellingProduct))
            {
                TopSellingProducts.Add(topSellingProduct);
            }
        }
        #endregion
    }
}
