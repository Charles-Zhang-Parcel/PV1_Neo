using Parcel.CoreEngine.Runtime;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Toolboxes.Special.Nodes;

namespace Parcel.Neo.Base.Toolboxes.Special
{
    public class SpecialToolbox : IToolboxDefinition
    {
        #region Interface
        public ToolboxNodeExport[] ExportNodes => [
            // Special - Specialized Graph Visualization
            new("Graph Stats", RuntimeNodeType.Method, typeof(GraphStats)),
            //new("Console Output", RuntimeNodeType.Method, typeof(object)), // With options to specify how many lines to show
            //new("Python Snippet", RuntimeNodeType.Method, typeof(object)), // With auto binding inputs and outputs
            //null, // Divisor line // Utility
            //new("Graph Attributes", RuntimeNodeType.Method, typeof(object)),
            //null, // Divisor line // Decoration
            //new("Header", RuntimeNodeType.Method, typeof(object)),
            //new("Text", RuntimeNodeType.Method, typeof(object)),
            //new("URL", RuntimeNodeType.Method, typeof(object)),
            //new("Image", RuntimeNodeType.Method, typeof(object)),
            //new("Markdown", RuntimeNodeType.Method, typeof(object)),
            //new("Audio", RuntimeNodeType.Method, typeof(object)),
            //new("Web Page", RuntimeNodeType.Method, typeof(object)),
            //new("Help Page", RuntimeNodeType.Method, typeof(object)),
            //null, // Divisor line // Others
            //new("Contact", RuntimeNodeType.Method, typeof(object)),
            //new("About", RuntimeNodeType.Method, typeof(object)),
        ];
        #endregion
    }
}