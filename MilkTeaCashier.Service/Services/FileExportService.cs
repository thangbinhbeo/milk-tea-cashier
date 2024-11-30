using iTextSharp.text;
using iTextSharp.text.pdf;
using MilkTeaCashier.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Service.Services
{
    public class FileExportService : IFileExportService
    {
        /// <summary>
        /// Exports the given data to a CSV file.
        /// </summary>
        /// <param name="data">The data to be exported.</param>
        /// <param name="fileName">The name of the output file.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ExportToCSVAsync(string data, string fileName)
        {
            try
            {
                var filePath = $"{fileName}.csv";
                using (var writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    await writer.WriteAsync(data);
                }
            }
            catch (IOException ex)
            {
                // Log the exception (optional) or handle appropriately
                throw new IOException("Failed to export data to CSV.", ex);
            }
        }

        /// <summary>
        /// Exports the given data to a PDF file.
        /// </summary>
        /// <param name="data">The data to be exported.</param>
        /// <param name="fileName">The name of the output file.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ExportToPDFAsync(string data, string fileName)
        {
            try
            {
                var filePath = $"{fileName}.pdf";

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    var document = new iTextSharp.text.Document();
                    PdfWriter.GetInstance(document, stream);

                    document.Open();
                    document.Add(new Paragraph(data));
                    document.Close();
                }

                await Task.CompletedTask;
            }
            catch (DocumentException ex)
            {
                throw new DocumentException("Failed to export data to PDF.", ex);
            }
            catch (IOException ex)
            {
                throw new IOException("Failed to export data to PDF.", ex);
            }
        }
    }
}
