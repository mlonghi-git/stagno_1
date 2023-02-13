using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.IO;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        XmlNode root = null;
        XmlDocument doc = null;
        FileInfo fileInfo = null;
        string folderPath = null;

        public MainWindow()
        {
            InitializeComponent();
            folderPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            // folder che contiene i file
            folderPath += "\\file";
            selectFileInFolder(folderPath);
        }

        protected void selectFileInFolder(string directory)
        {

            try
            {
                string[] files = Directory.GetFiles(directory);
                foreach (string file in files)
                {
                    fileListBox.Items.Add(Path.GetFileName(file));
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show("Folder not found");
            }


        }

        private void fileListBox_getXMLFromList(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string filePath = folderPath + "\\" + fileListBox.SelectedItem.ToString();
            if (File.Exists(filePath))
            {
                string content = File.ReadAllText(filePath);
                fileInfo = new FileInfo(filePath);
                root = this.createXml(content);
                this.createResGrid(root);
            }
            else
            {
                MessageBox.Show("file non trovato");
            }
        }
        protected XmlDocument createXml(string fileContent)
        {
            doc = new XmlDocument();
            doc.LoadXml(fileContent);
            return doc;
        }

        // dal root prende i vari res e genera i campi testo e input
        protected void createResGrid(XmlNode root)
        {
            // reset gridRes
            gridRes.ColumnDefinitions.Clear();
            gridRes.RowDefinitions.Clear();
            gridRes.Children.Clear();

            XmlNodeList nodeList = root.SelectNodes("descendant::Step/Res");
            gridRes.ColumnDefinitions.Add(new ColumnDefinition());
            gridRes.ColumnDefinitions.Add(new ColumnDefinition());
            gridRes.ColumnDefinitions.Add(new ColumnDefinition());

            // header number
            TextBlock labelHeaderNumber = new TextBlock();
            labelHeaderNumber.Text = "#";
            Grid.SetRow(labelHeaderNumber, 0);
            Grid.SetColumn(labelHeaderNumber, 0);
            gridRes.Children.Add(labelHeaderNumber);

            // header res
            TextBlock labelHeaderRes = new TextBlock();
            labelHeaderRes.Text = "Res";
            Grid.SetRow(labelHeaderRes, 0);
            Grid.SetColumn(labelHeaderRes, 1);
            gridRes.Children.Add(labelHeaderRes);

            // header serial
            TextBlock labelHeaderSerial = new TextBlock();
            labelHeaderSerial.Text = "Serial";
            Grid.SetRow(labelHeaderSerial, 0);
            Grid.SetColumn(labelHeaderSerial, 2);
            gridRes.Children.Add(labelHeaderSerial);

            RowDefinition gridRow = new RowDefinition();
            gridRow.Height = new GridLength(25);
            gridRes.RowDefinitions.Add(gridRow);

            int curRow = 1;
            foreach (XmlNode node in nodeList)
            {
                TextBlock labelNumber = new TextBlock();
                labelNumber.Text = curRow.ToString();
                Grid.SetRow(labelNumber, curRow);
                Grid.SetColumn(labelNumber, 0);
                gridRes.Children.Add(labelNumber);

                TextBlock labelRes = new TextBlock();
                labelRes.Text = node.FirstChild.InnerText;
                Grid.SetRow(labelRes, curRow);
                Grid.SetColumn(labelRes, 1);
                gridRes.Children.Add(labelRes);

                TextBox txtb = new TextBox();
                txtb.ToolTip = "...";
                txtb.Name = "serial_" + (curRow).ToString();
                Grid.SetRow(txtb, curRow);
                Grid.SetColumn(txtb, 2);
                gridRes.Children.Add(txtb);

                RowDefinition gridRow1 = new RowDefinition();
                gridRow1.Height = new GridLength(25);
                gridRes.RowDefinitions.Add(gridRow1);

                curRow++;

            }

            sendButton.Visibility = Visibility.Visible;
        }

        private void fillXmlWithSerialsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<TextBox> textBoxes = gridRes.Children.OfType<TextBox>().ToList();
                foreach (TextBox txtb in textBoxes)
                {
                    if (txtb.Name.StartsWith("serial_"))
                    {
                        var serialValue = txtb.Text;
                        XmlNode serialNode = doc.SelectSingleNode("//Serial");
                        XmlNode valueNode = serialNode.FirstChild;
                        if (valueNode.InnerText == "")
                        {
                            valueNode.InnerText = serialValue;
                        }
                        else
                        {
                            XmlElement newChild = doc.CreateElement("Value");
                            newChild.InnerText = serialValue.ToString();
                            serialNode.AppendChild(newChild);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            sendFile();
        }

        //sarà da modificare con invio su cartella di rete
        private void sendFile()
        {
            try
            {
                string fileNameWithoutExtension = fileInfo.Name.Replace(fileInfo.Extension, "");
                string newFileName = fileInfo.Directory + "\\" + fileNameWithoutExtension + "_" + DateTime.Now.ToString("yyyyMMddTHHmmss") + fileInfo.Extension;
                doc.Save(newFileName);
                MessageBox.Show("File " + newFileName + " salvato correttamente");
            }
            catch (XmlException)
            {
                MessageBox.Show("Errore salvataggio file");
            }
        }
    }
}
