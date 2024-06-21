using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Toolboxes.Logic.Nodes;
using System.Linq;

namespace Parcel.Neo.Base.Toolboxes.Logic
{
    public class LogicToolbox : IToolboxDefinition
    {
        #region Interface
        public ToolboxNodeExport[] ExportNodes => [
            // Functional
            new ToolboxNodeExport("Choose", typeof(Choose)),
            .. AutomaticNodes.Select(a => a == null ? null : new ToolboxNodeExport(a.NodeName, a))
        ];
        public AutomaticNodeDescriptor[] AutomaticNodes => [
            // Numerical
            new("> (Bigger Than)", [typeof(double), typeof(double)], typeof(double),
                objects => (double)objects[0] > (double)objects[1]),
            new("< (Smaller Than)", [typeof(double), typeof(double)], typeof(double),
                objects => (double)objects[0] < (double)objects[1]),
            null, // Divisor line // Boolean
            new("AND", [typeof(bool), typeof(bool)], typeof(bool),
                objects => (bool)objects[0] && (bool)objects[1]),
            new("OR", [typeof(bool), typeof(bool)], typeof(bool),
                objects => (bool)objects[0] || (bool)objects[1]),
        ];
        #endregion
    }
}