using System.Reflection;
using System.Text.RegularExpressions;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.DataTypes;

namespace Parcel.Neo.Base.Toolboxes.String
{
    public class StringToolbox : IToolboxDefinition
    {
        #region Interface
        public string ToolboxName => "String";
        public string ToolboxAssemblyFullName => Assembly.GetExecutingAssembly().FullName;
        public ToolboxNodeExport[] ExportNodes => new ToolboxNodeExport[]
        {
        };
        public AutomaticNodeDescriptor[] AutomaticNodes => new AutomaticNodeDescriptor[]
        {
            // Basic Query
            new AutomaticNodeDescriptor("String Length", new []{CacheDataType.String}, CacheDataType.Number,
                objects => ((string)objects[0]).Length),
            null, // Divisor line // Operations
            new AutomaticNodeDescriptor("Replace", new []{CacheDataType.String, CacheDataType.String, CacheDataType.String}, CacheDataType.String,
                objects => ((string)objects[0]).Replace((string)objects[1], (string)objects[2]))
            {
                InputNames = new []{ "Source", "Old Value", "New Value"}
            },
            new AutomaticNodeDescriptor("Reg Replace", new []{CacheDataType.String, CacheDataType.String, CacheDataType.String}, CacheDataType.String,
                objects => Regex.Replace((string)objects[0], (string)objects[1], (string)objects[2]))
            {
                InputNames = new []{ "Source", "Pattern", "Replacement"}
            },
        };
        #endregion
    }
}