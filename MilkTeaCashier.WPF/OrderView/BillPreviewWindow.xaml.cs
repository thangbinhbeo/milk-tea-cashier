using Microsoft.Win32;
using MilkTeaCashier.Data.DTOs.OrderDTO;
using MilkTeaCashier.Service.Services;
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

namespace MilkTeaCashier.WPF.OrderView
{
    /// <summary>
    /// Interaction logic for BillPreviewWindow.xaml
    /// </summary>
    public partial class BillPreviewWindow : Window
    {
        private readonly OrderService _service;
        private int _orderId;
        public BillPreviewWindow(int orderId)
        {
            _service ??= new OrderService();
            InitializeComponent();
            _orderId = orderId;
        }

        private void ExportPDFButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    DefaultExt = "pdf",
                    FileName = $"Bill_{_orderId}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.pdf"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;

                    _service.ExportBillToPdf(_orderId, filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while exporting to PDF: " + ex.Message);
            }
        }

        private async void PrintButton_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                await _service.PrintBillToPrinter(_orderId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while printing: " + ex.Message);
            }
        }
    }
}
