using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using WpfApp1.Utilities;

namespace WpfApp1;

/// <summary>
///   Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private XmlNode _root;
    private XmlDocument _doc;
    private FileInfo _fileInfo;
    private List<DateTime> _fileDateList = new List<DateTime>();
    private readonly string _fileFolderPath;
    private readonly string _outFolderPath;

    public MainWindow()
    {
        InitializeComponent();
        _fileFolderPath = DirectoryHelper.GetInputFolderPath();
        _outFolderPath = DirectoryHelper.GetOutputFolderPath();
        LoadFilesFromFolder(_fileFolderPath);

        var timer = new Timer(1 * 1000);
        timer.Elapsed += Timer_Elapsed;
        timer.Start();
    }

    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        LoadFilesFromFolder(_fileFolderPath);
    }


    private void LoadFilesFromFolder(string directoryPath)
    {
        SelectFileInFolder(directoryPath);
    }

    private void SelectFileInFolder(string directory)
    {
        try
        {
            var files = Directory.GetFiles(directory);
            if (files.Length <= 0)
                return;

            Array.Sort(files, (a, b) => File.GetCreationTime(b).CompareTo(File.GetCreationTime(a)));
            var latestFile = Path.GetFileName(files[0]);
            if (!FileAlreadyInList(files))
            {
                _fileDateList.Clear();
                _fileDateList.Add(File.GetCreationTime(files[0]));

                fileListBox.Dispatcher.Invoke(() =>
                {
                    fileListBox.Items.Clear();
                    fileListBox.Items.Add(latestFile);
                    ManualGetXml(latestFile);
                    Application.Current.MainWindow.Activate();
                });
            }

           
        }
        catch (DirectoryNotFoundException)
        {
            MessageBox.Show("Folder not found");
        }
    }

    private bool FileAlreadyInList(string[] files)
    {
        return _fileDateList.Any(x => x == File.GetCreationTime(files[0]));
    }

    private void ManualGetXml(string file)
    {
        var filePath = _fileFolderPath + "\\" + file;
        if (File.Exists(filePath))
        {
            var content = File.ReadAllText(filePath);
            _fileInfo = new FileInfo(filePath);
            _root = CreateXml(content);
            CreateResGrid(_root);
        }
        else
        {
            MessageBox.Show("File not found");
        }
    }
    private void FileListBox_getXMLFromList(object sender, MouseButtonEventArgs e)
    {
        var filePath = _fileFolderPath + "\\" + fileListBox.SelectedItem;
        if (File.Exists(filePath))
        {
            var content = File.ReadAllText(filePath);
            _fileInfo = new FileInfo(filePath);
            _root = CreateXml(content);
            CreateResGrid(_root);
        }
        else
        {
            MessageBox.Show("File not found");
        }
    }

    private XmlDocument CreateXml(string fileContent)
    {
        _doc = new XmlDocument();
        _doc.LoadXml(fileContent);
        return _doc;
    }

    // dal root prende i vari res e genera i campi testo e input
    private void CreateResGrid(XmlNode root)
    {
        // reset gridRes
        gridRes.ColumnDefinitions.Clear();
        gridRes.RowDefinitions.Clear();
        gridRes.Children.Clear();

        var headerSerialNode = root.SelectSingleNode("//Serial/Value");
        if (headerSerialNode != null) { TBox_WorkOrder.Text= headerSerialNode.InnerText; }

        var nodeList = root.SelectNodes("descendant::Step/Res");
        gridRes.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
        gridRes.ColumnDefinitions.Add(new ColumnDefinition());
        gridRes.ColumnDefinitions.Add(new ColumnDefinition());

        // header number
        var labelHeaderNumber = new TextBlock { Text = "#" };
        Grid.SetRow(labelHeaderNumber, 0);
        Grid.SetColumn(labelHeaderNumber, 0);
        gridRes.Children.Add(labelHeaderNumber);

        // header res
        var labelHeaderRes = new TextBlock { Text = "Res" };
        Grid.SetRow(labelHeaderRes, 0);
        Grid.SetColumn(labelHeaderRes, 1);
        gridRes.Children.Add(labelHeaderRes);

        // header serial
        var labelHeaderSerial = new TextBlock { 
            Text = "Serial" };
        Grid.SetRow(labelHeaderSerial, 0);
        Grid.SetColumn(labelHeaderSerial, 2);
        gridRes.Children.Add(labelHeaderSerial);

        gridRes.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });

        var curRow = 1;
        foreach (XmlNode node in nodeList)
        {
            var labelNumber = new TextBlock 
            { 
                Text = curRow.ToString(),
                FontSize = 20,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Grid.SetRow(labelNumber, curRow);
            Grid.SetColumn(labelNumber, 0);
            gridRes.Children.Add(labelNumber);

            var labelRes = new TextBlock
            {
                Text = node.FirstChild.InnerText,
                FontSize = 20,
                VerticalAlignment = VerticalAlignment.Center

            };
            Grid.SetRow(labelRes, curRow);
            Grid.SetColumn(labelRes, 1);
            gridRes.Children.Add(labelRes);

            var serialTextBox = new TextBox
            {
                ToolTip = "...",
                Name = "serial_" + curRow,
                FontSize = 20,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5, 5, 0, 0),
                
            };
            Grid.SetRow(serialTextBox, curRow);
            Grid.SetColumn(serialTextBox, 2);
            gridRes.Children.Add(serialTextBox);

            gridRes.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });

            curRow++;
        }

        sendButton.Visibility = Visibility.Visible;
        //Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.System) + Path.DirectorySeparatorChar + "osk.exe");

    }


    private void FillXmlWithSerialsButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!string.IsNullOrEmpty(TBox_OpID.Text))
            {
                var opIdTBox = TBox_OpID.Text;
                var serialNode = _doc.SelectSingleNode($"//Serial");
                var opNode = _doc.CreateElement("Op");
                opNode.InnerText = opIdTBox;
                serialNode.AppendChild(opNode); 
            }

            var textBoxes = gridRes.Children.OfType<TextBox>().ToList();
            foreach (var tBox in textBoxes)
            {
                if (!tBox.Name.StartsWith("serial_"))
                    continue;

                if (!string.IsNullOrEmpty(tBox.Text))
                {
                    var serialValue = tBox.Text;
                    var num = int.Parse(tBox.Name.Replace("serial_", ""));
                    var stepNode = _doc.SelectSingleNode($"//Step[Num='{num}']");
                    var serNode = _doc.CreateElement("Ser");
                    serNode.InnerText = serialValue;
                    stepNode.AppendChild(serNode);
                } 
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        SendFile();
    }

    //sarà da modificare con invio su cartella di rete
    private void SendFile()
    {
        try
        {
            var fileNameWithoutExtension = _fileInfo.Name.Replace(_fileInfo.Extension, "");
            var newFileName = Path.Combine(_outFolderPath, $"{fileNameWithoutExtension}{DateTime.Now:yyyyMMddTHHmmss}{_fileInfo.Extension}");

            //NetworkCredential credentials = new NetworkCredential("mirko", "Admin123!");
            System.IO.File.WriteAllText(newFileName, _doc.InnerXml);
            ResetInterface();
            // TODO 
            // eliminare file da cartella /in se serve
            MessageBox.Show("File saved");
            //_doc.Save(newFileName);
        }
        catch (XmlException)
        {
            MessageBox.Show("Error saving file.");
        }
    }

    private void ResetInterface()
    {
        gridRes.ColumnDefinitions.Clear();
        gridRes.RowDefinitions.Clear();
        gridRes.Children.Clear();
        TBox_OpID.Clear();
        TBox_WorkOrder.Text = "";
    }
}


