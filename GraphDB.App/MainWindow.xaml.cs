using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml;

using GraphDB.Constructor.Semantic;
using GraphDB.Contract.Core;
using GraphDB.Contract.Enum;
using GraphDB.Core;
using GraphDB.Tool;

namespace GraphDB.App
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private SemanticConstructor myGraph;

        public MainWindow()
        {
            InitializeComponent();
            //DataInit();
            
        }

        private void DataInit()
        {
            myGraph = new SemanticConstructor("Semantic.xml");
            myGraph.ImportData(ReadFile("Siemens.txt"));
        }

        public string ReadFile(string path)
        {
            string content;
            try
            {
                content = File.ReadAllText(path, Encoding.UTF8);
            }
            catch (Exception)
            {
                return null;
            }
            //创建网络
            return content;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConfigWindow winConfig = new ConfigWindow("Semantic.xml");
            winConfig.ShowDialog();
        }
    }
}
