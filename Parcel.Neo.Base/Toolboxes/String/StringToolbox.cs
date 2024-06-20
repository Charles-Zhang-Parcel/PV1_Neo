using System.Text.RegularExpressions;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.DataTypes;
using System.Linq;

namespace Parcel.Neo.Base.Toolboxes.String
{
    public class StringToolbox : IToolboxDefinition
    {
        #region Interface
        public string ToolboxName => "String";
        public ToolboxNodeExport?[]? ExportNodes => AutomaticNodes.Select(a => a == null ? null : new ToolboxNodeExport(a.NodeName, CoreEngine.Runtime.RuntimeNodeType.Method, a)).ToArray();

        public AutomaticNodeDescriptor?[] AutomaticNodes => [
            // Basic Query
            new("String Length", [CacheDataType.String], CacheDataType.Number,
                objects => ((string)objects[0]).Length),
            null, // Divisor line // Operations
            new("Replace", [CacheDataType.String, CacheDataType.String, CacheDataType.String], CacheDataType.String,
                objects => ((string)objects[0]).Replace((string)objects[1], (string)objects[2]))
            {
                InputNames = ["Source", "Old Value", "New Value"]
            },
            new("Reg Replace", [CacheDataType.String, CacheDataType.String, CacheDataType.String], CacheDataType.String,
                objects => Regex.Replace((string)objects[0], (string)objects[1], (string)objects[2]))
            {
                InputNames = ["Source", "Pattern", "Replacement"]
            },
        ];
        #endregion
    }
}