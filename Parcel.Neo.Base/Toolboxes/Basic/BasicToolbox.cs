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
            new("Comment", typeof(CommentNode)),
            new("Preview", typeof(PreviewNode)),
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
            new("Graph Reference", typeof(GraphReferenceNode)),
            new("Sub Graph", typeof(object)),
            null, // Divisor Line // Special (or consider moving them into "Annotation")
            // Special - Specialized Graph Visualization
            new("Graph Stats", typeof(GraphStats)),
            //new("Console Output", typeof(object)), // With options to specify how many lines to show
            //new("Python Snippet", typeof(object)), // With auto binding inputs and outputs
            //null, // Divisor line // Utility
            //new("Graph Attributes", typeof(object)),
            //null, // Divisor line // Decoration
            //new("Header", typeof(object)),
            //new("Text", typeof(object)),
            //new("URL", typeof(object)),
            //new("Image", typeof(object)),
            //new("Markdown", typeof(object)),
            //new("Audio", typeof(object)),
            //new("Web Page", typeof(object)),
            //new("Help Page", typeof(object)),
            //null, // Divisor line // Others
            //new("Contact", typeof(object)),
            //new("About", typeof(object)),
        };
        #endregion
    }
}