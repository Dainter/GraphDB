using System.Xml;

using GraphDB.Contract.Core;
using GraphDB.Core;

namespace GraphDB.Constructor.Semantic.Model
{
    public class Hanzi : Node
    {
        public Hanzi( string name ) : base( name ) {}
        public Hanzi( INode oriNode ) : base( oriNode ) {}
        public Hanzi( XmlElement xNode ) : base( xNode ) {}
    }
}