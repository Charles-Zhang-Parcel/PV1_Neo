using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Toolboxes.Math.Nodes;

namespace Parcel.Neo.Base.Toolboxes.Math
{
    public class MathToolbox : IToolboxDefinition
    {
        #region Interface
        public ToolboxNodeExport[] ExportNodes => [
            // Quick Access
            new ToolboxNodeExport("Calculator", typeof(Calculator)), // Simple math parsed string to number
            new ToolboxNodeExport("Expression", typeof(Expression)), // Save as Calculator but with a max of 9 variable number of inputs; Auto-replace with $1-$9 as variable names
        ];
        #endregion
    }
}