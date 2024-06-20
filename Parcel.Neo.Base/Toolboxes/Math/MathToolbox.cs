using System.Reflection;
using Parcel.CoreEngine.Runtime;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Toolboxes.Math.Nodes;

namespace Parcel.Neo.Base.Toolboxes.Math
{
    public class MathToolbox : IToolboxDefinition
    {
        #region Interface
        public ToolboxNodeExport[] ExportNodes => [
            // Quick Access
            new ToolboxNodeExport("Calculator", RuntimeNodeType.Method, typeof(Calculator)), // Simple math parsed string to number
            new ToolboxNodeExport("Expression", RuntimeNodeType.Method, typeof(Expression)), // Save as Calculator but with a max of 9 variable number of inputs; Auto-replace with $1-$9 as variable names
            null, // Divisor line // Basic Numberical Operations
            new ToolboxNodeExport("Add", RuntimeNodeType.Method, typeof(Add)),
            new ToolboxNodeExport("Subtract", RuntimeNodeType.Method, typeof(Subtract)),
            new ToolboxNodeExport("Multiply", RuntimeNodeType.Method, typeof(Multiply)),
            new ToolboxNodeExport("Divide", RuntimeNodeType.Method, typeof(Divide)),
            new ToolboxNodeExport("Modulus", RuntimeNodeType.Method, typeof(Module)),
            new ToolboxNodeExport("Power", RuntimeNodeType.Method, typeof(Power)),
            null, // Divisor line // Math Functions
            new ToolboxNodeExport("Sin", RuntimeNodeType.Method, typeof(Sin)),
        ];
        #endregion
    }
}