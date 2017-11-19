using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

using GraphDB.Contract.Core;
using GraphDB.Contract.Serial;
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

            //INode nodeA = new Node(Guid.NewGuid().ToString(), "NodeA");
            //myNodeList.Add(nodeA.Guid, nodeA);
            //INode nodeB = new Node(Guid.NewGuid().ToString(), "NodeB");
            //myNodeList.Add(nodeB.Guid, nodeB);
            //IEdge edgeA = new Edge("LinkA");
            //edgeA.From = nodeA;
            //edgeA.To = nodeB;
            //nodeA.AddEdge(edgeA);
            //nodeB.RegisterInbound(edgeA);
            //myEdgeList.Add(edgeA);
            //SaveDataBase();
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
                //Add Link
                AddEdge(newEdge.FromGuid, newEdge.ToGuid, newEdge);
            }
            return;
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

        //加入节点
        private void AddNode(INode newNode)
        {
            if( newNode == null )
            {
                return;
            }
            //节点加入节点列表
            myNodeList.Add(newNode.Guid, newNode);
        }

        //删除节点 by Guid
        private void RemoveNode(string guid)
        {
            if (guid == null)
            {
                return;
            }
            INode curNode = myNodeList[ guid ];
            if( curNode == null )
            {
                return;
            }
            RemoveNode( curNode );
        }

        //删除节点 by Node
        private void RemoveNode(INode curNode)
        {
            //清除节点所有连边
            ClearUnusedEdge(curNode.ClearEdge());
            //从节点列表中移除节点
            myNodeList.Remove(curNode.Guid);
        }

        //加入连边 by Guid
        private void AddEdge( string curNodeGuid, string tarNodeGuid, IEdge newEdge )
        {
            if (curNodeGuid == null || tarNodeGuid == null || newEdge == null)
            {
                return;
            }
            INode curNode = myNodeList[curNodeGuid];
            if (curNode == null)
            {
                return;
            }
            INode tarNode = myNodeList[tarNodeGuid];
            if (tarNode == null)
            {
                return;
            }
            AddEdge( curNode, tarNode, newEdge);
        }

        //加入连边 by Node
        private void AddEdge( INode curNode, INode tarNode, IEdge newEdge )
        {
            //连边的头指针指向起节点
            newEdge.From = curNode;
            //连边的尾指针指向目标节点
            newEdge.To = tarNode;
            //将新连边加入起始节点的outbound
            if (curNode.AddEdge(newEdge) == false)
            {
                return;
            }
            //将新连边加入目标节点的Inbound
            if (tarNode.RegisterInbound(newEdge) == false)
            {
                return;
            }
            //全部完成后将连边加入网络连边列表
            myEdgeList.Add(newEdge);
        }

        //移除连边 by Guid
        private void RemoveEdge(string curNodeGuid, string tarNodeGuid, string attribute)
        {
            if (curNodeGuid == null || tarNodeGuid == null || attribute == null)
            {
                return;
            }
            INode curNode = myNodeList[curNodeGuid];
            if (curNode == null)
            {
                return;
            }
            INode tarNode = myNodeList[tarNodeGuid];
            if (tarNode == null)
            {
                return;
            }
            RemoveEdge(curNode, tarNode, attribute);
        }

        //移除连边 by Node
        private void RemoveEdge(INode curNode, INode tarNode, string attribute)
        {
            //从起始节点的出边中遍历,查找终止节点编号和目标节点编号和类型一致的连边
            IEdge curEdge = curNode.OutBound.First(x => x.To.Guid == tarNode.Guid && x.Attribute == attribute);
            if (curEdge == null)
            {//没找到直接返回
                return;
            }
            //起始节点Outbound中移除连边
            curNode.RemoveEdge(curEdge);
            //从终止节点InBound中注销连边
            tarNode.UnRegisterInbound(curEdge);
            //全部完成后，从总连边列表中移除该边
            myEdgeList.Remove(curEdge);
        }

        //删除所有节点和连边
        public void ClearAll()
        {
            myEdgeList.Clear();
            myNodeList.Clear();
        }

        //删除所有被解除绑定的连边
        private void ClearUnusedEdge(List<IEdge> unusedList)
        {
            if (unusedList == null)
            {
                return;
            }
            //将入参列表中所有连边从总连边列表中删除
            foreach (IEdge edge in unusedList)
            {
                myEdgeList.Remove(edge);
            }
            //清空入参列表本身内容
            unusedList.Clear();
        }

        //删除所有连边
        public void ClearAllEdge()
        {
            myEdgeList.Clear();
        }

    }
}