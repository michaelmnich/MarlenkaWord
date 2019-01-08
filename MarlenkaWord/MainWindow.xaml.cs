using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;


namespace MarlenkaWord
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private List<string> Cache;
        private List<string> Cache_Y;
        private int chachecounter;
        public int CacheLimit=10;

        private int chachecounter_save;
        public int CacheLimit_save = 20;
        public MainWindow()
        {
            InitializeComponent();
            Cache = new List<string>();
            Cache_Y = new List<string>();
            //  rtbEditor.KeyDown += new KeyEventHandler(textBox1_KeyDown);
            string pathLocal = Directory.GetCurrentDirectory();
            pathLocal = Path.Combine(pathLocal, "dump");
            string[] files = Directory.GetFiles(pathLocal);

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.LastAccessTime < DateTime.Now.AddDays(-3));
                    fi.Delete();
            }


        }


        private string DataSaver_helper()
        {
            DateTime time = DateTime.Now;
            string f = time.ToString("yyyy_MM_dd-HH_mm") + ".ml";
            string pathLocal = Directory.GetCurrentDirectory();
            pathLocal = Path.Combine(pathLocal, "dump");
            string LocalFile = Path.Combine(pathLocal,f);
            return LocalFile;
        }


        public void DataSaver()
        {

            string LocalFile = DataSaver_helper();

            //if (!File.Exists(LocalFile))
            //{
            //    File.Create(LocalFile);
                
            //}
            string createText = GetText();
            File.WriteAllText(LocalFile, createText);
          
        }


        private  string GetText()
        {
            return new TextRange(rtbEditor.Document.ContentStart,
                rtbEditor.Document.ContentEnd).Text;
        }

        private void MemoryWorker()
        {
            if (chachecounter > CacheLimit)
            {
                Cache.Add(GetText());
                Cache_Y.Clear();
                chachecounter = 0;
            }

            if (chachecounter_save > CacheLimit_save)
            {
                DataSaver();
                chachecounter_save = 0;
            }
            chachecounter++;
            chachecounter_save++;
        }


      
      
        private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {

            MemoryWorker();
            TextRange tempRange = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Selection.Start);
            TextRange tempRange2 = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            int alltext = tempRange2.Text.Length - 2;
            if (alltext < 0) alltext = 0;
            txtStatus.Text = "ilość znaków: " + alltext;
            txtStatus.Text += ", kursor na znaku: " + tempRange.Text.Length +"." + Environment.NewLine;
            txtStatus.Text += "Zaznaczono: " + rtbEditor.Selection.Text.Length + " znaków ";
        }

        private void Back_OnClick(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            int id = Cache.Count - 1;
            if (id <= Cache.Count && id >= 0)
            {
                textRange.Text = Cache[id];
                Cache_Y.Add(Cache[Cache.Count-1]);
                Cache.RemoveAt(Cache.Count-1);
            }
            
        }

        private void Forward_OnClick(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            int id = Cache_Y.Count - 1;
            if (id <= Cache_Y.Count && id >= 0)
            {
                textRange.Text = Cache_Y[id];
                Cache.Add(Cache_Y[Cache_Y.Count - 1]);
                Cache_Y.RemoveAt(Cache_Y.Count - 1);
            }
        }

        private void Restore_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.Filter = "Marlenka File System (*.ml)|*.ml";
            if (openFileDialog.ShowDialog() == true)
            {
                this.popup.Visibility = Visibility.Visible;
                txtEditor.Text = File.ReadAllText(openFileDialog.FileName);
            }
                

        }

        private void TxtNotOk_OnClick(object sender, RoutedEventArgs e)
        {
            this.popup.Visibility = Visibility.Collapsed;
        }

        private void TxtOK_OnClick(object sender, RoutedEventArgs e)
        {
            this.popup.Visibility = Visibility.Collapsed;
            TextRange textRange = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
            textRange.Text = txtEditor.Text;
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Marlenka File System (*.ml)|*.ml";
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, textRange.Text);
        }
    }
}
