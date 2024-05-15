using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
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


namespace Tubes3_apaKek
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private BitmapImage _inputImage;
        private string _executionTime;
        private string _similarityPercentage;
        private string _selectedAlgorithm = "BM";

        public BitmapImage InputImage
        {
            get => _inputImage;
            set
            {
                _inputImage = value;
                OnPropertyChanged(nameof(InputImage));
            }
        }

        public string ExecutionTime
        {
            get => _executionTime;
            set
            {
                if (_executionTime != value)
                {
                    _executionTime = value;
                    OnPropertyChanged(nameof(ExecutionTime));
                }
            }
        }

        public string SimilarityPercentage
        {
            get => _similarityPercentage;
            set
            {
                if (_similarityPercentage != value)
                {
                    _similarityPercentage = value;
                    OnPropertyChanged(nameof(SimilarityPercentage));
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string SelectedAlgorithm
        {
            get => _selectedAlgorithm;
            set
            {
                if (_selectedAlgorithm != value)
                {
                    _selectedAlgorithm = value;
                    OnPropertyChanged(nameof(SelectedAlgorithm));
                }
            }
        }

        private void OnInsertImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                DefaultExt = ".png",
                Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                try
                {
                    var uri = new Uri(dlg.FileName);
                    InputImage = new BitmapImage(uri);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load image: " + ex.Message);
                }
            }
        }

        private void OnSelectBM(object sender, RoutedEventArgs e)
        {
            if (((RadioButton)sender).IsChecked == true)
            {
                SelectedAlgorithm = "BM";
            }
        }

        private void OnSelectKMP(object sender, RoutedEventArgs e)
        {
            if (((RadioButton)sender).IsChecked == true)
            {
                SelectedAlgorithm = "KMP";
            }
        }


        private void OnSearch(object sender, RoutedEventArgs e)
        {
            // Simulate a search operation
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Simulate the time taken for searching (mock delay)
            System.Threading.Thread.Sleep(20); // Simulate processing time

            stopwatch.Stop();

            // Update the UI with mock results and include the selected algorithm in the output
            ExecutionTime = $"Waktu Pencarian: {stopwatch.ElapsedMilliseconds} ms using {SelectedAlgorithm}";
            SimilarityPercentage = "Persentase Kecocokkan: 72%";
        }

    }
}