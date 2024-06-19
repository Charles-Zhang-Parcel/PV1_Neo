using System.Reflection;
using Parcel.Neo.Base.Framework;
using Parcel.Toolbox.FileSystem.Nodes;

namespace Parcel.Toolbox.FileSystem
{
    public class FileSystemToolbox: IToolboxDefinition
    {
        #region Interface
        public string ToolboxName => "File System";
        public string ToolboxAssemblyFullName => Assembly.GetExecutingAssembly().FullName;
        public ToolboxNodeExport[] ExportNodes => new ToolboxNodeExport[]
        {
            // Basic IO
            new ToolboxNodeExport("Read File", typeof(object)),
            new ToolboxNodeExport("Read File as Number", typeof(object)),
            new ToolboxNodeExport("Read File as Dictionary", typeof(object)),
            // new ToolboxNodeExport("Read File as List", typeof(object)), // Don't do this, it's just one step away the same as CSV
            null, // Divisor line // Save File
            new ToolboxNodeExport("Write CSV", typeof(WriteCSV)), // Preview should open file location
            new ToolboxNodeExport("Write String", typeof(object)),
            new ToolboxNodeExport("Write Number", typeof(object)),
        };
        public AutomaticNodeDescriptor[] AutomaticNodes => new AutomaticNodeDescriptor[] { };
        #endregion
    }
}