using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilkTeaCashier.Service.Interfaces
{
    public interface IDialogService
    {
        void ShowMessage(string message, string title);
        void ShowError(string message, Exception exception);
    }
}
