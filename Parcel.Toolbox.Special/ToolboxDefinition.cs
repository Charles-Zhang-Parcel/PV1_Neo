using System.Reflection;
using Parcel.Neo.Shared.Framework;
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
            new ToolboxNodeExport("Graph Stats", typeof(GraphStats)),
            new ToolboxNodeExport("Console Output", typeof(object)), // With options to specify how many lines to show
            new ToolboxNodeExport("Python Snippet", typeof(object)), // With auto binding inputs and outputs
            new ToolboxNodeExport("Host Address", typeof(HostAddress)),
            null, // Divisor line // Utility
            new ToolboxNodeExport("Graph Attributes", typeof(object)),
            null, // Divisor line // Decoration
            new ToolboxNodeExport("Header", typeof(object)),
            new ToolboxNodeExport("Text", typeof(object)),
            new ToolboxNodeExport("URL", typeof(object)),
            new ToolboxNodeExport("Image", typeof(object)),
            new ToolboxNodeExport("Markdown", typeof(object)),
            new ToolboxNodeExport("Audio", typeof(object)),
            new ToolboxNodeExport("Web Page", typeof(object)),
            new ToolboxNodeExport("Help Page", typeof(object)),
            null, // Divisor line // Others
            new ToolboxNodeExport("Contact", typeof(object)),
            new ToolboxNodeExport("About", typeof(object)),
        };
        public AutomaticNodeDescriptor[] AutomaticNodes => new AutomaticNodeDescriptor[] { };
        #endregion
    }
}