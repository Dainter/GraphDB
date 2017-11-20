using System;
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
        private Graph myGraph;

        public MainWindow()
        {
            InitializeComponent();
            DataInit();
        }

        private void DataInit()
        {
            //myGraph = new Graph("db.xml");

            myGraph = new Graph();
            INode nodeA = new Task( "NodeC");
            myGraph.Nodes.Add(nodeA.Guid, nodeA);
            INode nodeB = new Task( "NodeD");
            myGraph.Nodes.Add(nodeB.Guid, nodeB);
            IEdge edgeA = new RelateTo();
            edgeA.From = nodeA;
            edgeA.To = nodeB;
            nodeA.AddEdge(edgeA);
            nodeB.RegisterInbound(edgeA);
            myGraph.Edges.Add(edgeA);
            myGraph.SaveDataBase();

        }
    }
}
