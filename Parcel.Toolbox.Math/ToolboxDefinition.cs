using System.Reflection;
using Parcel.Neo.Base.Framework;
using Parcel.Toolbox.Math.Nodes;

namespace Parcel.Toolbox.Math
{
    public class ToolboxDefinition: IToolboxDefinition
    {
        #region Interface
        public string ToolboxName => "Math";
        public string ToolboxAssemblyFullName => Assembly.GetExecutingAssembly().FullName;
        public ToolboxNodeExport[] ExportNodes => new[]
        {
            // Quick Access
            new ToolboxNodeExport("Calculator", typeof(Calculator)), // Simple math parsed string to number
            new ToolboxNodeExport("Expression", typeof(Expression)), // Save as Calculator but with a max of 9 variable number of inputs; Auto-replace with $1-$9 as variable names
            null, // Divisor line // Basic Numberical Operations
            new ToolboxNodeExport("Add", typeof(Add)),
            new ToolboxNodeExport("Subtract", typeof(Subtract)),
            new ToolboxNodeExport("Multiply", typeof(Multiply)),
            new ToolboxNodeExport("Divide", typeof(Divide)),
            new ToolboxNodeExport("Modulus", typeof(Module)),
            new ToolboxNodeExport("Power", typeof(Power)),
            null, // Divisor line // Math Functions
            new ToolboxNodeExport("Sin", typeof(Sin)),
        };
        public AutomaticNodeDescriptor[] AutomaticNodes => new AutomaticNodeDescriptor[] { };
        #endregion
    }
}