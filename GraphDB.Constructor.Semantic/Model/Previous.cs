using System.Xml;

using GraphDB.Constructor.Semantic.Utility;
using GraphDB.Core;

namespace GraphDB.Constructor.Semantic.Model
{
    public class Previous : Edge
    {
        public Previous( string newValue = "1" ) : base(CommonStrings.Previous, newValue ) {}
        public Previous( XmlElement xNode ) : base( xNode ) {}

        public void AddWeight()
        {
            int value;
            if (!int.TryParse(Value, out value))
            {
                Value = "1";
            }
            Value = (value + 1).ToString();
        }
    }
}