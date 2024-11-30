using MilkTeaCashier.Data.UnitOfWork;
using MilkTeaCashier.Service.Interfaces;
using MilkTeaCashier.Service.Services;
using MilkTeaCashier.WPF.ViewModels;
using MilkTeaCashier.WPF.Views;
using System.Configuration;
using System.Data;
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

        }

    }

}
