using iTextSharp.text;
using iTextSharp.text.pdf;
using MilkTeaCashier.Service.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Service.Services
{
    public class FileExportService : IFileExportService
    {
        private readonly IDialogService _dialogService;
        private readonly IFileDialogService _fileDialogService;
        public FileExportService(IDialogService dialogService, IFileDialogService fileDialogService)
        {
            _dialogService = dialogService;
            _fileDialogService = fileDialogService;
        }

        public async Task ExportToCSVAsync(string data, string defaultFileName)
        {
            try
            {
                var filePath = GetSaveFilePath($"{defaultFileName}.csv", "CSV Files|*.csv");
                if (string.IsNullOrEmpty(filePath)) return;

                using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    await writer.WriteAsync(data);
                }

                _dialogService.ShowMessage($"CSV file has been exported successfully to:\n{filePath}", "Export Successful");
            }
            catch (IOException ex)
            {
                _dialogService.ShowError("Failed to export data to CSV. Please try again.", ex);
            }
        }

        public async Task ExportToPDFAsync(string data, string defaultFileName)
        {
            try
            {
                var filePath = GetSaveFilePath($"{defaultFileName}.pdf", "PDF Files|*.pdf");
                if (string.IsNullOrEmpty(filePath)) return;

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    var document = new Document();
                    PdfWriter.GetInstance(document, stream);

                    document.Open();
                    document.Add(new Paragraph(data));
                    document.Close();
                }

                _dialogService.ShowMessage($"PDF file has been exported successfully to:\n{filePath}", "Export Successful");
            }
            catch (DocumentException ex)
            {
                _dialogService.ShowError("Failed to export data to PDF. Please try again.", ex);
            }
            catch (IOException ex)
            {
                _dialogService.ShowError("Failed to export data to PDF. Please try again.", ex);
            }
        }

        private string GetSaveFilePath(string defaultFileName, string filter)
        {
            return _fileDialogService.GetSaveFilePath(defaultFileName, filter);
        }
    }
}
