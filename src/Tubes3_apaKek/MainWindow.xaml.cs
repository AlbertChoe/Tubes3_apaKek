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
using Services.Hash;

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
        private BitmapImage _matchedFingerprint;
        public MainWindow()
        {
            InitializeComponent();
            Database.GetAllFingerprintPaths();
            DataContext = this;
            // TestDatabaseFunctions();
        }
        public BitmapImage InputImage
        {
            get => _inputImage;
            set
            {
                _inputImage = value;
                OnPropertyChanged(nameof(InputImage));
            }
        }

        public BitmapImage MatchedFingerprint
        {
            get => _matchedFingerprint;
            set
            {
                _matchedFingerprint = value;
                OnPropertyChanged(nameof(MatchedFingerprint));
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
            ResultData? result = null;
            List<string> allpaths = Database.GetAllFingerprintPaths();


            await Task.Run(() =>
            {
                result = Logic.Search(this.SelectedAlgorithm, this._inputImage, allpaths);
                if (result == null)
                {
                    result = Logic.LDController(this._inputImage, allpaths);
                }
            });

            if (result != null)
            {
                DisplayMatchedFingerprintImage(result.fingerprintImagePath);
                Biodata biodata = result.biodata;
                BiodataResults = $"Nama: {biodata.Nama}\nNIK: {biodata.NIK}\nTempat Lahir: {biodata.TempatLahir}\nTanggal Lahir: {biodata.TanggalLahir.ToShortDateString()}\nJenis Kelamin: {biodata.JenisKelamin}\nGolongan Darah: {biodata.GolonganDarah}\nAlamat: {biodata.Alamat}\nAgama: {biodata.Agama}\nStatus Perkawinan: {biodata.StatusPerkawinan}\nPekerjaan: {biodata.Pekerjaan}\nKewarganegaraan: {biodata.Kewarganegaraan}";
                ExecutionTime = $"Waktu Pencarian: {result.execTime} ms \nAlgorithm: {result.algorithm}";
                SimilarityPercentage = $"Persen Kecocokan: {result.Similarity:F1}%";
            }
            else
            {
                ClearResultsDisplay();
            }

            loadingPopup.IsOpen = false;
            SetButtonsEnabled(true);
        }

      private void DisplayMatchedFingerprintImage(string relativeImagePath)
        {
            try
            {

                string rootDirectory = Directory.GetCurrentDirectory(); 
                string fullPath = System.IO.Path.Combine(rootDirectory, relativeImagePath); // Combine the root with the relative image path
                var uri = new Uri(fullPath, UriKind.Absolute);
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = uri;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();  // Make the BitmapImage thread-safe
                MatchedFingerprint = bitmap; 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load matched fingerprint image: {ex.Message}");
            }
        }

        private void ClearResultsDisplay()
        {
            BiodataResults = "No Results Found";
            SimilarityPercentage = "";
            ExecutionTime = "";
            MatchedFingerprint = null; 
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
            // Blowfish.tes();
            btnBM.Background = new SolidColorBrush(Colors.Red);
            btnKMP.Background = new SolidColorBrush(Colors.Gray);
            SelectedAlgorithm = "BM";
        }
    }
}