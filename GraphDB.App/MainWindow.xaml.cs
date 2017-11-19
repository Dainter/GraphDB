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
            //INode nodeA = new Task(Guid.NewGuid().ToString(), "NodeC");
            //myGraph.Nodes.Add(nodeA.Guid, nodeA);
            //INode nodeB = new Task(Guid.NewGuid().ToString(), "NodeD");
            //myGraph.Nodes.Add(nodeB.Guid, nodeB);
            //IEdge edgeA = new RelateTo("LinkA");
            //edgeA.From = nodeA;
            //edgeA.To = nodeB;
            //nodeA.AddEdge(edgeA);
            //nodeB.RegisterInbound(edgeA);
            //myGraph.Edges.Add(edgeA);
            //myGraph.SaveDataBase();

        }
    }
}
