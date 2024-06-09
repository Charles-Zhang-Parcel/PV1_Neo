using System.Reflection;
using Parcel.Neo.Base.Framework;
using Parcel.Toolbox.Special.Nodes;

namespace Parcel.Toolbox.Special
{
    public class ToolboxDefinition: IToolboxEntry
    {
        #region Interface
        public string ToolboxName => "Special";
        public string ToolboxAssemblyFullName => Assembly.GetExecutingAssembly().FullName;
        public ToolboxNodeExport[] ExportNodes => new ToolboxNodeExport[] 
        {
            // Special - Specialized Graph Visualization
            new("Graph Stats", typeof(GraphStats)),
            new("Console Output", typeof(object)), // With options to specify how many lines to show
            new("Python Snippet", typeof(object)), // With auto binding inputs and outputs
            null, // Divisor line // Utility
            new("Graph Attributes", typeof(object)),
            null, // Divisor line // Decoration
            new("Header", typeof(object)),
            new("Text", typeof(object)),
            new("URL", typeof(object)),
            new("Image", typeof(object)),
            new("Markdown", typeof(object)),
            new("Audio", typeof(object)),
            new("Web Page", typeof(object)),
            new("Help Page", typeof(object)),
            null, // Divisor line // Others
            new("Contact", typeof(object)),
            new("About", typeof(object)),
        };
        public AutomaticNodeDescriptor[] AutomaticNodes => System.Array.Empty<AutomaticNodeDescriptor>();
        #endregion
    }
}