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
            //new("Read File", typeof(object)),
            //new("Read File as Number", typeof(object)),
            //new("Read File as Dictionary", typeof(object)),
            // new("Read File as List", typeof(object)), // Don't do this, it's just one step away the same as CSV
            null, // Divisor line // Save File
            new("Write CSV", typeof(WriteCSV)), // Preview should open file location
            //new("Write String", typeof(object)),
            //new("Write Number", typeof(object)),
        };
        #endregion
    }
}