using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
namespace TTFTypeFace
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
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
            {
                using(FileStream stream = new FileStream(openFileDialog.FileName,FileMode.Open))
                {
                    byte[] data = new byte[stream.Length];
                    stream.Read(data);
                    TrueTypeFont.TTFTypeFace tTFTypeFace = new TrueTypeFont.TTFTypeFace(data);

                    tTFTypeFace.Dispose();
                }
            }
        }
    }
}
