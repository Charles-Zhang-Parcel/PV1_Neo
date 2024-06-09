using System.Reflection;
using Parcel.Neo.Shared.Framework;

namespace Parcel.Toolbox.Scripting
{
    public class ToolboxDefinition: IToolboxEntry
    {
        #region Interface
        public string ToolboxName => "Basic";
        public string ToolboxAssemblyFullName => Assembly.GetExecutingAssembly().FullName;
        public ToolboxNodeExport[] ExportNodes => new ToolboxNodeExport[]
        {
            new ToolboxNodeExport("REPL", typeof(object)),
            null, // Divisor line // Python
            new ToolboxNodeExport("Python Snippet", typeof(object)),
        };
        public AutomaticNodeDescriptor[] AutomaticNodes => new AutomaticNodeDescriptor[] { };
        #endregion
    }
}