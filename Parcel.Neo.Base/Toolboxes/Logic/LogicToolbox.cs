using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Toolboxes.Logic.Nodes;
using Parcel.Neo.Base.DataTypes;
using System.Linq;

namespace Parcel.Neo.Base.Toolboxes.Logic
{
    public class LogicToolbox : IToolboxDefinition
    {
        #region Interface
        public ToolboxNodeExport[] ExportNodes => [
            // Functional
            new ToolboxNodeExport("Choose", CoreEngine.Runtime.RuntimeNodeType.Method, typeof(Choose)),
            .. AutomaticNodes.Select(a => a == null ? null : new ToolboxNodeExport(a.NodeName, CoreEngine.Runtime.RuntimeNodeType.Method, a))
        ];
        public AutomaticNodeDescriptor[] AutomaticNodes => [
            // Numerical
            new("> (Bigger Than)", [CacheDataType.Number, CacheDataType.Number], CacheDataType.Number,
                objects => (double)objects[0] > (double)objects[1]),
            new("< (Smaller Than)", [CacheDataType.Number, CacheDataType.Number], CacheDataType.Number,
                objects => (double)objects[0] < (double)objects[1]),
            null, // Divisor line // Boolean
            new("AND", [CacheDataType.Boolean, CacheDataType.Boolean], CacheDataType.Boolean,
                objects => (bool)objects[0] && (bool)objects[1]),
            new("OR", [CacheDataType.Boolean, CacheDataType.Boolean], CacheDataType.Boolean,
                objects => (bool)objects[0] || (bool)objects[1]),
        ];
        #endregion
    }
}