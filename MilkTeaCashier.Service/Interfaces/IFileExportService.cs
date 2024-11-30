using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Service.Interfaces
{
    public interface IFileExportService
    {
        Task ExportToCSVAsync(string data, string fileName);
        Task ExportToPDFAsync(string data, string fileName);
    }
}
