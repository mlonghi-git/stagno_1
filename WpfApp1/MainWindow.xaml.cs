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
        string PATH_TO_FOLDER = "C:\\Users\\mlonghi\\source\\repos\\WpfApp1\\WpfApp1\\file";
        //string FILE_NAME = null;

        public MainWindow()
        {
            InitializeComponent();
            selectFileInFolder(PATH_TO_FOLDER);
        }

        protected void selectFileInFolder(string directory)
        {

            try
            {
                // recupera file della cartella
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
            string filePath = PATH_TO_FOLDER + "\\" + fileListBox.SelectedItem.ToString();
            if(File.Exists(filePath))
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
        protected void Button_Click(Object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            Nullable<bool> result = openFileDlg.ShowDialog(); 
            
            if (result == true)
            {
                FileNameTextBox.Text = openFileDlg.FileName;
                this.createResGrid(root);
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


            Button fillXmlWithSerialsButton = new Button();
            fillXmlWithSerialsButton.Content = "invio";
            fillXmlWithSerialsButton.Click += fillXmlWithSerialsButton_Click;
            gridRes.Children.Add(fillXmlWithSerialsButton);

        }
        protected void fillXmlWithSerialsButton_Click(object sender, EventArgs e)
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
                        if(valueNode.InnerText == "")
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
                
                // salvato per dopo per salvare file
                //Microsoft.Win32.SaveFileDialog saveFileDlg = new Microsoft.Win32.SaveFileDialog();
                //Nullable<bool> result = saveFileDlg.ShowDialog();
                //if (result == true)
                //{
                //    root.OwnerDocument.Save(saveFileDlg.FileName);
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            

            
        }

        
    }
}
