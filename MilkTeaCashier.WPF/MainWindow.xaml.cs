using Microsoft.Win32;
using MilkTeaCashier.Service.Services;
using MilkTeaCashier.WPF.ViewModels;
using MilkTeaCashier.WPF.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MilkTeaCashier.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.OpenReportViewCommand.Execute(null);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            return;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			ProductView productwindow = new ProductView();

			productwindow.Show();
		}

	}
}