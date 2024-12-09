using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Service.Interfaces
{
    public interface IFileDialogService
    {
        /// <summary>
        /// Opens a Save File Dialog and returns the selected file path.
        /// </summary>
        /// <param name="defaultFileName">Default file name for the save file dialog.</param>
        /// <param name="filter">File filter (e.g., "CSV Files|*.csv").</param>
        /// <returns>The selected file path, or null if canceled.</returns>
        string GetSaveFilePath(string defaultFileName, string filter);
    }
}
