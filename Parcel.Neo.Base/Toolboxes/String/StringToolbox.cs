using System.Text.RegularExpressions;
using Parcel.Neo.Base.Framework;
using System.Linq;

namespace Parcel.Neo.Base.Toolboxes.String
{
    public class StringToolbox : IToolboxDefinition
    {
        #region Interface
        public ToolboxNodeExport?[]? ExportNodes => AutomaticNodes.Select(a => a == null ? null : new ToolboxNodeExport(a.NodeName, a)).ToArray();

        public AutomaticNodeDescriptor?[] AutomaticNodes => [
            // Basic Query
            new("String Length", [typeof(string)], typeof(double),
                objects => ((string)objects[0]).Length),
            null, // Divisor line // Operations
            new("Replace", [typeof(string), typeof(string), typeof(string)], typeof(string),
                objects => ((string)objects[0]).Replace((string)objects[1], (string)objects[2]))
            {
                InputNames = ["Source", "Old Value", "New Value"]
            },
            new("Reg Replace", [typeof(string), typeof(string), typeof(string)], typeof(string),
                objects => Regex.Replace((string)objects[0], (string)objects[1], (string)objects[2]))
            {
                InputNames = ["Source", "Pattern", "Replacement"]
            },
        ];
        #endregion
    }
}