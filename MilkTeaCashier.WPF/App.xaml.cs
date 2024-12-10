using MilkTeaCashier.Data.UnitOfWork;
using MilkTeaCashier.Service.Interfaces;
using MilkTeaCashier.Service.Services;
using MilkTeaCashier.WPF.ViewModels;
using MilkTeaCashier.WPF.Views;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace MilkTeaCashier.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Redirect console output to Debug
            Console.SetOut(new DebugTextWriter());
        }

    }
    public class DebugTextWriter : System.IO.TextWriter
    {
        public override void Write(char value)
        {
            Debug.Write(value);
        }

        public override void Write(string value)
        {
            Debug.Write(value);
        }

        public override void WriteLine(string value)
        {
            Debug.WriteLine(value);
        }

        public override Encoding Encoding => Encoding.Default;
    }
}
