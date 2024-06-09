using System.Reflection;
using Parcel.Neo.Shared.Framework;
using Parcel.Toolbox.Present.Nodes;

namespace Parcel.Toolbox.Present
{
    public class ToolboxDefinition: IToolboxEntry
    {
        #region Interface
        public string ToolboxName => "Present";
        public string ToolboxAssemblyFullName => Assembly.GetExecutingAssembly().FullName;
        public ToolboxNodeExport[] ExportNodes => new ToolboxNodeExport[] 
        {
            new ToolboxNodeExport("Present", typeof(PresentOnline)),
            null, // Divisor line
            new ToolboxNodeExport("Header", typeof(object)),
            new ToolboxNodeExport("Page", typeof(Page)),
            new ToolboxNodeExport("Section", typeof(Section)),
            new ToolboxNodeExport("Panel", typeof(object)),
            new ToolboxNodeExport("Paragraph", typeof(object)),
            null, // Advanced Layout
            // ...
        };
        public AutomaticNodeDescriptor[] AutomaticNodes => new AutomaticNodeDescriptor[] { };
        #endregion
    }
}