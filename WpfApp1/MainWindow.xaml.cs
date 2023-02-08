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
using System.Xml;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        XmlNode root = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        protected void Button_Click(Object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();

            // Launch OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = openFileDlg.ShowDialog(); 
            // Get the selected file name and display in a TextBox.
            // Load content of file in a TextBlock
            if (result == true)
            {
                FileNameTextBox.Text = openFileDlg.FileName;
                //TextBlock1.Text = System.IO.File.ReadAllText(openFileDlg.FileName);
                XmlNode root = this.createXml(openFileDlg);
                this.createResGrid(root);

                //XmlNode myNode = root.SelectSingleNode("descendant::Serial/Value");
                //myNode.InnerText = "blabla";
                //TextBlock1.Text = doc.InnerXml.ToString();
            }
        }

        protected XmlNode createXml(Microsoft.Win32.OpenFileDialog file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file.FileName);
            root = doc.DocumentElement;
            return root;
        }

        // dal root prende i vari res e genera i campi testo e input
        protected void createResGrid(XmlNode root)
        {
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
                        var TBD = txtb.Text;
                    }
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
