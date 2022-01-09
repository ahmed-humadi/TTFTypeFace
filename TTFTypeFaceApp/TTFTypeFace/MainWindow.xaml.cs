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
        private ScaleTransform scaleTransform = new ScaleTransform(1, -1);
        public MainWindow()
        {
            InitializeComponent();
            CustomPanel.LayoutTransform = scaleTransform;
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
                    if (tTFTypeFace.MagicNumber == 0x5F0F3CF5)
                    {
                        double x = 0; double y = CustomPanel.ActualHeight - 30;
                        for (ushort i = 0; i < tTFTypeFace.NumberOfGlyphs; i++)
                        {
                            Geometry glyph = tTFTypeFace.GetGlyphOutline(i);

                            x = x + 20;

                            if (x > 650)
                            { y -= 30; x = 20; }

                            glyph.Transform = new TranslateTransform(x, y);
                            using (DrawingContext dc = CustomPanel.RenderOpen())
                            {
                                dc.DrawGeometry(Brushes.Black, null, glyph);
                            }
                        }
                    }
                    else
                        MessageBox.Show("is not True Type Font");
                    tTFTypeFace.Dispose();
                }
            }
        }
    }
}
