using Microsoft.Win32;
using MilkTeaCashier.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MilkTeaCashier.WPF.Utilities
{
    class DialogHelper : IDialogService, IFileDialogService
    {
        /// <summary>
        /// Shows a message dialog with a specified message and title.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="title">The title of the dialog.</param>
        public void ShowMessage(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Shows an error dialog with a specified message and exception details.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        /// <param name="exception">The exception that occurred.</param>
        public void ShowError(string message, Exception exception)
        {
            var detailedMessage = $"{message}\n\nDetails:\n{exception.Message}";
            MessageBox.Show(detailedMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public string GetSaveFilePath(string defaultFileName, string filter)
        {
            var saveFileDialog = new SaveFileDialog
            {
                FileName = defaultFileName,
                Filter = filter,
                Title = "Save File",
                OverwritePrompt = true
            };

            return saveFileDialog.ShowDialog() == true ? saveFileDialog.FileName : null;
        }
    }
}
