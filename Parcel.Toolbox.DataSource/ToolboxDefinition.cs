using System.Reflection;
using Parcel.Neo.Base.Framework;
using Parcel.Toolbox.DataSource.Nodes;

namespace Parcel.Toolbox.DataSource
{
    public class ToolboxDefinition: IToolboxDefinition
    {
        #region Interface
        public string ToolboxName => "Data Source";
        public string ToolboxAssemblyFullName => Assembly.GetExecutingAssembly().FullName;
        public ToolboxNodeExport[] ExportNodes => new[]
        {
            // Data Base System
            new ToolboxNodeExport("MS MDL", typeof(object)),
            new ToolboxNodeExport("PL SQL", typeof(object)),
            null, // Divisor line // Web Services
            new ToolboxNodeExport("Yahoo Finance", typeof(YahooFinance)),
        };
        public AutomaticNodeDescriptor[] AutomaticNodes => new AutomaticNodeDescriptor[] { };
        #endregion
    }
}