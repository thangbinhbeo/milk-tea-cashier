using MilkTeaCashier.Data.UnitOfWork;
using MilkTeaCashier.Service.Services;
using MilkTeaCashier.WPF.Utilities;
using MilkTeaCashier.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MilkTeaCashier.WPF.Views
{
    /// <summary>
    /// Interaction logic for ReportView.xaml
    /// </summary>
    public partial class ReportView : Window
    {
        public ReportView()
        {
            InitializeComponent();
            var dialogService = new DialogHelper();
            var reportingService = new ReportingService(new UnitOfWork()); // Replace with actual DI
            var fileExportService = new FileExportService(dialogService, dialogService);
            DataContext = new ReportViewModel(reportingService, fileExportService);
        }
    }
}
