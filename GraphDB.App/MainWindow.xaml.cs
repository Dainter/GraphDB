using System.Windows;

using GraphDB.Contract.Core;
using GraphDB.Core;

namespace GraphDB.App
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DataInit()
        {
            IEdge edgeA = new Edge( "Link");
        }
    }
}
