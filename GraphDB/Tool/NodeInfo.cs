using System.ComponentModel;

using GraphDB.Contract.Core;

namespace GraphDB.Tool
{
    class NodeInfo:INotifyPropertyChanged
    {
        readonly string myNodeName;
        readonly string myNodeType;

        public string Name => myNodeName;

        public string Type => myNodeType;

        public NodeInfo()
        {
            myNodeName = "";
            myNodeType = "";
        }

        public NodeInfo(INode oriNode)
        {
            myNodeName = string.Copy(oriNode.Name);
            myNodeType = string.Copy(oriNode.GetType().Name);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void Notify(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

    }
}
