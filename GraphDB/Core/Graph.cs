using System;
using System.Collections.Generic;
using System.Xml;

using GraphDB.Contract;
using GraphDB.Contract.Core;
using GraphDB.IO;
using GraphDB.Utility;

namespace GraphDB.Core
{
    public class Graph
    {
        private readonly Dictionary<string, INode> myNodeList;
        private readonly List<IEdge> myEdgeList;
        private readonly IIoStrategy myIohandler;

        public Dictionary<string, INode> Nodes => myNodeList;

        public List<IEdge> Edges => myEdgeList;

        public Graph()
        {
            myNodeList = new Dictionary<string, INode>();
            myEdgeList = new List<IEdge>();
            myIohandler = new XMLStrategy("db.xml");

            INode nodeA = new Node(Guid.NewGuid().ToString(), "NodeA");
            myNodeList.Add(nodeA.Guid, nodeA);
            INode nodeB = new Node(Guid.NewGuid().ToString(), "NodeB");
            myNodeList.Add(nodeB.Guid, nodeB);
            IEdge edgeA = new Edge("LinkA");
            edgeA.From = nodeA;
            edgeA.To = nodeB;
            nodeA.AddEdge(edgeA);
            nodeB.RegisterInbound(edgeA);
            myEdgeList.Add(edgeA);
            SaveDataBase();
        }

        public Graph( string path )
        {
            ErrorCode err;
            myNodeList = new Dictionary<string, INode>();
            myEdgeList = new List<IEdge>();
            myIohandler = new XMLStrategy(path);

            XmlElement graph = myIohandler.ReadFile(out err);
            if( err != ErrorCode.NoError )
            {
                throw new Exception( $"Error found during read DB file. Error Code:{err}" );
            }
            var nodes = graph.GetNode(XmlNames.Nodes);
            var edges = graph.GetNode(XmlNames.Edges);

            //Nodes
            foreach (XmlElement curItem in nodes)
            {
                INode newNode = (INode)SerializableHelper.Deserialize(curItem);
                if (newNode == null)
                {
                    throw new Exception($"Error found during Deserialize. XML:{curItem}");
                }
                myNodeList.Add(newNode.Guid, newNode);
            }
            //Edges
            foreach (XmlElement curItem in edges)
            {
                IEdge newEdge = (IEdge)SerializableHelper.Deserialize(curItem);
                if (newEdge == null)
                {
                    throw new Exception($"Error found during Deserialize. XML:{curItem}");
                }
                myEdgeList.Add(newEdge);
            }
            //Add Link
        }

        public void SaveDataBase()
        {
            ErrorCode err;
            XmlDocument doc = ToXML();
            myIohandler.SaveFile(doc, out err);
            if (err != ErrorCode.NoError)
            {
                throw new Exception($"Error found during save DB file. Error Code:{err}");
            }
        }

        //将数据保存为XML文件（接口）
        public XmlDocument ToXML()
        {
            //所有网络数据都保存为xml格式
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement(XmlNames.Graph);

            //Nodes
            var nodes = doc.CreateElement(XmlNames.Nodes);
            nodes.SetAttribute(XmlNames.NodeNumber, myNodeList.Count.ToString());
            foreach (KeyValuePair<string, INode> curItem in myNodeList)
            {
                XmlElement newNode = SerializableHelper.Serialize(doc, curItem.Value);
                nodes.AppendChild(newNode);     
            }
            root.AppendChild(nodes);
            //Edges
            var edges = doc.CreateElement(XmlNames.Edges);
            edges.SetAttribute(XmlNames.EdgeNumber, myEdgeList.Count.ToString());
            foreach (IEdge curItem in myEdgeList)
            {
                edges.AppendChild(SerializableHelper.Serialize(doc, curItem)); 
            }
            root.AppendChild(edges);

            doc.AppendChild(root);
            return doc;
        }
    }
}