using System.Reflection;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.Advanced;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Base.Framework.ViewModels.Primitives;
using Parcel.Neo.Base.Toolboxes.Basic.Nodes;

namespace Parcel.Neo.Base.Toolboxes.Basic
{
    public class ToolboxDefinition : IToolboxDefinition
    {
        #region Interface
        public string ToolboxName => "Basic";
        public string ToolboxAssemblyFullName => Assembly.GetExecutingAssembly().FullName!;
        public ToolboxNodeExport?[] ExportNodes => new ToolboxNodeExport?[]
        {
            new("Comment", typeof(CommentNode)),
            new("Preview", typeof(Preview)),
            null, // Divisor line // Primitive Nodes
            new("Number", typeof(NumberNode)),
            new("String", typeof(StringNode)),
            new("Boolean", typeof(BooleanNode)),
            new("DateTime", typeof(DateTimeNode)),
            new("Text", typeof(object)),
            new("File", typeof(OpenFileNode)),
            new("Save File", typeof(object)),
            // new("Array", typeof(object)), // Generic array representation of all above types, CANNOT have mixed types
            null, // Divisor line // Graph Modularization
            new("Graph Input", typeof(GraphInput)),
            new("Graph Output", typeof(GraphOutput)),
            new("Graph Reference", typeof(GraphReference)),
            new("Sub Graph", typeof(object)),
        };
        public AutomaticNodeDescriptor[] AutomaticNodes => System.Array.Empty<AutomaticNodeDescriptor>();
        #endregion
    }
}