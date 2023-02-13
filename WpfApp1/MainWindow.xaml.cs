using System;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
    private readonly string _fileFolderPath;
    private readonly string _outFolderPath
        ;

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

            fileListBox.Dispatcher.Invoke(() => {
                fileListBox.Items.Clear();
                fileListBox.Items.Add(latestFile);
            });
        }
        catch (DirectoryNotFoundException)
        {
            MessageBox.Show("Folder not found");
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

        var nodeList = root.SelectNodes("descendant::Step/Res");
        gridRes.ColumnDefinitions.Add(new ColumnDefinition());
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
        var labelHeaderSerial = new TextBlock { Text = "Serial" };
        Grid.SetRow(labelHeaderSerial, 0);
        Grid.SetColumn(labelHeaderSerial, 2);
        gridRes.Children.Add(labelHeaderSerial);

        gridRes.RowDefinitions.Add(new RowDefinition { Height = new GridLength(25) });

        var curRow = 1;
        foreach (XmlNode node in nodeList)
        {
            var labelNumber = new TextBlock { Text = curRow.ToString() };
            Grid.SetRow(labelNumber, curRow);
            Grid.SetColumn(labelNumber, 0);
            gridRes.Children.Add(labelNumber);

            var labelRes = new TextBlock { Text = node.FirstChild.InnerText };
            Grid.SetRow(labelRes, curRow);
            Grid.SetColumn(labelRes, 1);
            gridRes.Children.Add(labelRes);

            var serialTextBox = new TextBox
            {
                ToolTip = "...",
                Name = "serial_" + curRow
            };
            Grid.SetRow(serialTextBox, curRow);
            Grid.SetColumn(serialTextBox, 2);
            gridRes.Children.Add(serialTextBox);

            gridRes.RowDefinitions.Add(new RowDefinition { Height = new GridLength(25) });

            curRow++;
        }

        sendButton.Visibility = Visibility.Visible;
    }


    private void FillXmlWithSerialsButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var textBoxes = gridRes.Children.OfType<TextBox>().ToList();
            foreach (var tBox in textBoxes)
            {
                if (!tBox.Name.StartsWith("serial_"))
                    continue;

                var serialValue = tBox.Text;
                var num = int.Parse(tBox.Name.Replace("serial_", ""));
                var stepNode = _doc.SelectSingleNode($"//Step[Num='{num}']");
                var serialNode = _doc.CreateElement("Serial");
                serialNode.InnerText = serialValue;
                stepNode.AppendChild(serialNode);
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
            _doc.Save(newFileName);
        }
        catch (XmlException)
        {
            MessageBox.Show("Error saving file.");
        }
    }
}