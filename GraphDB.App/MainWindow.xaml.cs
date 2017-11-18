using System.Windows;

using GraphDB.Core;

namespace GraphDB.App
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Graph myGraph;

        public MainWindow()
        {
            InitializeComponent();
            DataInit();
        }

        private void DataInit()
        {
            myGraph = new Graph("db.xml");

            //myGraph = new Graph();


        }
    }
}
