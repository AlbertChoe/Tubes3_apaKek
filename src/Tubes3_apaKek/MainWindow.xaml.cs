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
using Tubes3_apaKek.DataAccess;
using Services;
using Tubes3_apaKek.Models;
using System.IO;


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
        private string _selectedAlgorithm = "KMP";
        private string _biodataResults;
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

        public string BiodataResults
        {
            get { return _biodataResults; }
            set
            {
                _biodataResults = value;
                OnPropertyChanged("BiodataResults");
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
            // TestDatabaseFunctions();
        }


        private void TestDatabaseFunctions()
        {
            // Test untuk mendapatkan semua path dari sidik jari
            var paths = Database.GetAllFingerprintPaths();
            // File.WriteAllText("pat.txt", paths[0]);
            MessageBox.Show("INI TESTING FUNGSI DOANG DI MAINWINDOWS.XAML.CS\n KLO FUNGSI CONTROLLER DI DATABASE.CS \n" + "Paths:\n" + string.Join("\n", paths));

            // Test untuk mendapatkan nama asli berdasarkan path
            if (paths.Count > 0)
            {
                var realName = Database.GetRealNameByPath(paths[0]); // mengasumsikan paths tidak kosong

                MessageBox.Show("INI TESTING FUNGSI DOANG DI MAINWINDOWS.XAML.CS\n KLO FUNGSI CONTROLLER DI DATABASE.CS \n" + "Real Name for " + paths[0] + ":\n" + realName);
            }

            // Test untuk mendapatkan semua nama alay
            var alayNames = Database.GetAllAlayNames();
            MessageBox.Show("INI TESTING FUNGSI DOANG DI MAINWINDOWS.XAML.CS\n KLO FUNGSI CONTROLLER DI DATABASE.CS \n" + "Alay Names:\n" + string.Join("\n", alayNames));

            // Test untuk mendapatkan biodata berdasarkan nama alay
            if (alayNames.Count > 0)
            {
                var biodata = Database.GetBiodataByRealName(alayNames[0]); // mengasumsikan alayNames tidak kosong
                MessageBox.Show("INI TESTING FUNGSI DOANG DI MAINWINDOWS.XAML.CS\n KLO FUNGSI CONTROLLER DI DATABASE.CS \n" + "Biodata for " + alayNames[0] + ":\n" + biodata?.ToString()); // ToString harus diimplementasikan di Biodata
            }
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
                DefaultExt = ".bmp",
                Filter = "BMP Files (*.bmp)|*.bmp|All Files (*.*)|*.*"
            };

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                try
                {
                    var uri = new Uri(dlg.FileName);
                    BitmapImage bitmap = new BitmapImage();

                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; // Ensure the image is fully loaded
                    bitmap.UriSource = uri;
                    bitmap.EndInit();
                    bitmap.Freeze(); // Make the BitmapImage thread-safe

                    InputImage = bitmap;
                    this._inputImage = bitmap;
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


       private async void OnSearch(object sender, RoutedEventArgs e)
        {
            SetButtonsEnabled(false);

            loadingPopup.IsOpen = true;

            ResultData result = null;

            // Jalankan pencarian di thread terpisah untuk menghindari UI freeze
            await Task.Run(() =>
            {
                result = Logic.Search(this.SelectedAlgorithm, this._inputImage);
            });

            // Proses hasil pencarian
            if (result != null)
            {
                Biodata biodata = result.biodata;
                BiodataResults = $"Nama: {biodata.Nama}\nNIK: {biodata.NIK}\nTempat Lahir: {biodata.TempatLahir}\nTanggal Lahir: {biodata.TanggalLahir.ToShortDateString()}\nJenis Kelamin: {biodata.JenisKelamin}\nGolongan Darah: {biodata.GolonganDarah}\nAlamat: {biodata.Alamat}\nAgama: {biodata.Agama}\nStatus Perkawinan: {biodata.StatusPerkawinan}\nPekerjaan: {biodata.Pekerjaan}\nKewarganegaraan: {biodata.Kewarganegaraan}";
                ExecutionTime = $"Waktu Pencarian: {result.execTime} ms \nAlgorithm: {result.algorithm}";
                SimilarityPercentage = $"Persentase Kecocokan: {result.Similarity}%";
            }
            else
            {
                BiodataResults = "Not Found";
            }

            loadingPopup.IsOpen = false;

            SetButtonsEnabled(true);
        }
        private void SetButtonsEnabled(bool isEnabled)
        {
            btnInsert.IsEnabled = isEnabled;
            btnKMP.IsEnabled = isEnabled;
            btnBM.IsEnabled = isEnabled;
            btnSearch.IsEnabled = isEnabled;
        }

        private void btnKMP_Click(object sender, RoutedEventArgs e)
        {
            btnKMP.Background = new SolidColorBrush(Colors.Red);
            btnBM.Background = new SolidColorBrush(Colors.Gray);
            SelectedAlgorithm = "KMP";
        }

        private void btnBM_Click(object sender, RoutedEventArgs e)
        {
            btnBM.Background = new SolidColorBrush(Colors.Red);
            btnKMP.Background = new SolidColorBrush(Colors.Gray);
            SelectedAlgorithm = "BM";
        }
    }
}