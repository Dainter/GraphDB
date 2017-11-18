
namespace GraphDB.Contract.Core
{
    public interface IEdge
    {
        INode From { get; set; }
        INode To { get;  set;  }
        string Attribute { get; }
        string Value { get; set; }
    }
}