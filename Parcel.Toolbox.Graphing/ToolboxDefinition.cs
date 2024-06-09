using System.Reflection;
using Parcel.Neo.Shared.Framework;
using Parcel.Toolbox.Graphing.Nodes;

namespace Parcel.Toolbox.Graphing
{
    public class ToolboxDefinition: IToolboxEntry
    {
        #region Interface
        public string ToolboxName => "Graphing";
        public string ToolboxAssemblyFullName => Assembly.GetExecutingAssembly().FullName;
        public ToolboxNodeExport[] ExportNodes => new ToolboxNodeExport[] 
        {
            new ToolboxNodeExport("Line Chart", typeof(LineChart)),
            new ToolboxNodeExport("Bar Chart", typeof(object)),
            new ToolboxNodeExport("Data Table", typeof(object)),
            new ToolboxNodeExport("Tree Map", typeof(TreeMap)),
        };
        public AutomaticNodeDescriptor[] AutomaticNodes => new AutomaticNodeDescriptor[] { };
        #endregion
    }
}