using Microsoft.Win32;
using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

обробник кнопки btnClear
<Window x:Class = "WpfViewImages.MainWindow"
        xmlns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns: x = "http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns: d = "http://schemas.microsoft.com/expression/blend/2008"
        xmlns: mc = "http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns: local = "clr-namespace:WpfViewImages"
        mc: Ignorable = "d"
        Title = "Перегляд зображень" Height = "500" Width = "720"
        WindowStartupLocation = "CenterScreen" >
    < Grid Margin = "10" >
        < Grid.RowDefinitions >
            < RowDefinition Height = "Auto" />
            < RowDefinition Height = "*" />
            < RowDefinition Height = "Auto" />
        </ Grid.RowDefinitions >
        < WrapPanel Margin = "0,0,0,10" >
            < Button x: Name = "btnOpen" Content = "Вибрати зображення..."
                    Click = "btnOpen_Click"
                    Padding = "12,6" />
            < Button Name = "btnClear" Content = "Очистити"
                    Padding = "12,6" Margin = "8,0,0,0" />
        </ WrapPanel >
        < Border Grid.Row = "1" BorderBrush = "Gray" BorderThickness = "1"
                Background = "#FFF5F5F5" AllowDrop = "True" Drop = "Border_Drop" >
            < Image Name = "imgPreview" Stretch = "Uniform"
                   RenderOptions.BitmapScalingMode = "HighQuality" />
        </ Border >
        < TextBlock Name = "txtInfo" Grid.Row = "2" Margin = "0,10,0,0"
                   Text = "Файл не вибрано. Можна також перетягнути зображення у вікно." />
    </ Grid >
</ Window >


using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace WpfViewImages
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

        private void Border_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    LoadImage(files[0]);
                }
            }
        }
        //Потрібно завантажити фото у вікно, щоб воно
        //було лише в пам'яті - Щоб фото не було заблокованим
        private void LoadImage(string path)
        {
            try
            {
                //bitmap - для роботи з фото
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.EndInit();
                bitmap.Freeze(); //у нашому bitmap - фото - path
                imgPreview.Source = bitmap;
                var sizeKb = new FileInfo(path).Length / 1024.0;
                txtInfo.Text = $"{Path.GetFileName(path)} — {bitmap.PixelWidth}×{bitmap.PixelHeight} px, {sizeKb:F1} КБ";

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося завантажити фото: \n{ex.Message}",
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Виберіть зображення",
                Filter = "Зображення|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tif;*.tiff|Усі файли|*.*",
                CheckFileExists = true,
            };
            if (dialog.ShowDialog() == true)
            {
                LoadImage(dialog.FileName);
            }
        }
    }
}