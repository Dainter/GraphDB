using System.Xml;

using GraphDB.Core;

namespace GraphDB.App
{
    public class RelateTo : Edge
    {
        public RelateTo( string newValue = "1" ) : base( "RelateTo", newValue ) {}
        public RelateTo( XmlElement xNode ) : base( xNode ) {}
    }
}