using Parcel.CoreEngine.Runtime;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Toolboxes.FileSystem.Nodes;

namespace Parcel.Neo.Base.Toolboxes.FileSystem
{
    public class FileSystemToolbox : IToolboxDefinition
    {
        #region Interface
        public ToolboxNodeExport[] ExportNodes => new ToolboxNodeExport[]
        {
            // Basic IO
            //new("Read File", RuntimeNodeType.Method, typeof(object)),
            //new("Read File as Number", RuntimeNodeType.Method, typeof(object)),
            //new("Read File as Dictionary", RuntimeNodeType.Method, typeof(object)),
            // new("Read File as List", RuntimeNodeType.Method, typeof(object)), // Don't do this, it's just one step away the same as CSV
            null, // Divisor line // Save File
            new("Write CSV", RuntimeNodeType.Method, typeof(WriteCSV)), // Preview should open file location
            //new("Write String", RuntimeNodeType.Method, typeof(object)),
            //new("Write Number", RuntimeNodeType.Method, typeof(object)),
        };
        #endregion
    }
}