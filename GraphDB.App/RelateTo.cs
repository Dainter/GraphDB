using System.Xml;

using GraphDB.Core;

namespace GraphDB.App
{
    public class RelateTo : Edge
    {
        public RelateTo( string newAttribute, string newValue = "1" ) : base( newAttribute, newValue ) {}
        public RelateTo( XmlElement xNode ) : base( xNode ) {}
    }
}