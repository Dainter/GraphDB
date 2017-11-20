using System.Xml;

using GraphDB.Contract.Core;
using GraphDB.Contract.Serial;
using GraphDB.Core;
using GraphDB.Utility;

namespace GraphDB.App
{
    public class Task : Node
    {
        private readonly string myTitle;

        [Serializable]
        public string Title => myTitle;

        public Task( string name ) : base( name )
        {
            myTitle = "Title Test";
        }

        public Task( INode oriNode ) : base( oriNode )
        {
            myTitle = "Title Test";
        }

        public Task( XmlElement xNode ) : base( xNode )
        {
            myTitle = xNode.GetText("Title");
        }
    }
}