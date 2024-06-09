using System.Reflection;
using Parcel.Neo.Shared.Framework;
using Parcel.Toolbox.Plotting.Nodes;

namespace Parcel.Toolbox.Plotting
{
    public class ToolboxDefinition: IToolboxEntry
    {
        #region Interface
        public string ToolboxName => "Plotting";
        public string ToolboxAssemblyFullName => Assembly.GetExecutingAssembly().FullName;
        public ToolboxNodeExport[] ExportNodes => new ToolboxNodeExport[] 
        {
            new ToolboxNodeExport("Plot", typeof(Plot)),
        };
        public AutomaticNodeDescriptor[] AutomaticNodes => new AutomaticNodeDescriptor[] { };
        #endregion
    }
}