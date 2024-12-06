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
        private string _selectedFilePath;
        private readonly ImageUploadService _firebaseService;

        public MainWindow()
        {
            InitializeComponent();
            _firebaseService = new ImageUploadService();
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
            CategoryManagement categoryWindow = new CategoryManagement();

            categoryWindow.Show();
        }

        private async void SelectAndUploadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Chọn ảnh để upload"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedFilePath = openFileDialog.FileName;
            }

            try
            {
                string fileName = System.IO.Path.GetFileName(_selectedFilePath);
                string url = await _firebaseService.UploadImageAsync(_selectedFilePath, fileName);

                ResultText.Text = $"Upload Successful! File URL: {url}";
            }
            catch (Exception ex)
            {
                ResultText.Text = $"Error: {ex.Message}";
            }
        }
    }
}