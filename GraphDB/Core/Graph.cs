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

        //�����ݱ���ΪXML�ļ����ӿڣ�
        public XmlDocument ToXML()
        {
            //�����������ݶ�����Ϊxml��ʽ
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

        //����ڵ�
        private void AddNode(INode newNode)
        {
            if( newNode == null )
            {
                return;
            }
            //�ڵ����ڵ��б�
            myNodeList.Add(newNode.Guid, newNode);
        }

        //ɾ���ڵ� by Guid
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

        //ɾ���ڵ� by Node
        private void RemoveNode(INode curNode)
        {
            //����ڵ���������
            ClearUnusedEdge(curNode.ClearEdge());
            //�ӽڵ��б����Ƴ��ڵ�
            myNodeList.Remove(curNode.Guid);
        }

        //�������� by Guid
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

        //�������� by Node
        private void AddEdge( INode curNode, INode tarNode, IEdge newEdge )
        {
            //���ߵ�ͷָ��ָ����ڵ�
            newEdge.From = curNode;
            //���ߵ�βָ��ָ��Ŀ��ڵ�
            newEdge.To = tarNode;
            //�������߼�����ʼ�ڵ��outbound
            if (curNode.AddEdge(newEdge) == false)
            {
                return;
            }
            //�������߼���Ŀ��ڵ��Inbound
            if (tarNode.RegisterInbound(newEdge) == false)
            {
                return;
            }
            //ȫ����ɺ����߼������������б�
            myEdgeList.Add(newEdge);
        }

        //�Ƴ����� by Guid
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

        //�Ƴ����� by Node
        private void RemoveEdge(INode curNode, INode tarNode, string attribute)
        {
            //����ʼ�ڵ�ĳ����б���,������ֹ�ڵ��ź�Ŀ��ڵ��ź�����һ�µ�����
            IEdge curEdge = curNode.OutBound.First(x => x.To.Guid == tarNode.Guid && x.Attribute == attribute);
            if (curEdge == null)
            {//û�ҵ�ֱ�ӷ���
                return;
            }
            //��ʼ�ڵ�Outbound���Ƴ�����
            curNode.RemoveEdge(curEdge);
            //����ֹ�ڵ�InBound��ע������
            tarNode.UnRegisterInbound(curEdge);
            //ȫ����ɺ󣬴��������б����Ƴ��ñ�
            myEdgeList.Remove(curEdge);
        }

        //ɾ�����нڵ������
        public void ClearAll()
        {
            myEdgeList.Clear();
            myNodeList.Clear();
        }

        //ɾ�����б�����󶨵�����
        private void ClearUnusedEdge(List<IEdge> unusedList)
        {
            if (unusedList == null)
            {
                return;
            }
            //������б����������ߴ��������б���ɾ��
            foreach (IEdge edge in unusedList)
            {
                myEdgeList.Remove(edge);
            }
            //�������б�������
            unusedList.Clear();
        }

        //ɾ����������
        public void ClearAllEdge()
        {
            myEdgeList.Clear();
        }

    }
}