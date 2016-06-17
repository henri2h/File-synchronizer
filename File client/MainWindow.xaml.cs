using ConsoleClient;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace File_client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string fileName;
        public MainWindow()
        {
            InitializeComponent();

           
        }



        private void tbText_TextChanged(object sender, TextChangedEventArgs e)
        {
           // MessageBox.Show("hey");
        }
        public void start()
        {
            string ip = "192.168.0.36";
            TCPDialog.start(ip);

            string fileedit = "";
            foreach (string filelist in TCPDialog.listFiles())
            {
                fileedit = filelist;
            }
            fileName = fileedit;
            MemoryStream file = TCPDialog.recieveFile(fileedit);
            if (file != null) { System.Diagnostics.Debug.WriteLine("transfert ended"); }
             else { System.Diagnostics.Debug.WriteLine("transfert failed"); }

            string fileContent = Encoding.UTF8.GetString(file.ToArray());
            tbText.Text = fileContent;
          //  bool disconnection = TCPDialog.disconnect();
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            start();
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            byte[] content =   Encoding.UTF8.GetBytes(tbText.Text);
            MemoryStream fileStream = new MemoryStream();
            fileStream.Write(content, 0, content.Length);
            TCPDialog.sendFile(fileName, fileStream);
        }
    }
}
