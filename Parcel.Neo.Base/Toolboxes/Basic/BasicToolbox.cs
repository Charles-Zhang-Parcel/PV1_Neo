using Parcel.CoreEngine.Runtime;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.Advanced;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Base.Framework.ViewModels.Primitives;
using Parcel.Neo.Base.Toolboxes.Basic.Nodes;

namespace Parcel.Neo.Base.Toolboxes.Basic
{
    public class BasicToolbox : IToolboxDefinition
    {
        #region Interface
        public ToolboxNodeExport?[] ExportNodes => new ToolboxNodeExport?[]
        {
            new("Comment", RuntimeNodeType.Method, typeof(CommentNode)),
            new("Preview", RuntimeNodeType.Method, typeof(PreviewNode)),
            null, // Divisor line // Primitive Nodes
            new("Number", RuntimeNodeType.Method, typeof(NumberNode)),
            new("String", RuntimeNodeType.Method, typeof(StringNode)),
            new("Boolean", RuntimeNodeType.Method, typeof(BooleanNode)),
            new("DateTime", RuntimeNodeType.Method, typeof(DateTimeNode)),
            new("Text", RuntimeNodeType.Method, typeof(object)),
            new("File", RuntimeNodeType.Method, typeof(OpenFileNode)),
            new("Save File", RuntimeNodeType.Method, typeof(object)),
            // new("Array", RuntimeNodeType.Method, typeof(object)), // Generic array representation of all above types, CANNOT have mixed types
            null, // Divisor line // Graph Modularization
            new("Graph Input", RuntimeNodeType.Method, typeof(GraphInput)),
            new("Graph Output", RuntimeNodeType.Method, typeof(GraphOutput)),
            new("Graph Reference", RuntimeNodeType.Method, typeof(GraphReferenceNode)),
            new("Sub Graph", RuntimeNodeType.Method, typeof(object)),
        };
        #endregion
    }
}