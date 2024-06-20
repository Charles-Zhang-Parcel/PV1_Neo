using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Toolboxes.Finance.Nodes;

namespace Parcel.Neo.Base.Toolboxes.Finance
{
    public class FinanceToolbox : IToolboxDefinition
    {
        #region Interface
        public ToolboxNodeExport[] ExportNodes => new ToolboxNodeExport[]
        {
            // Basic - Operations on Columns (Those will check and validate column type as Number/Double)
            new("Mean", CoreEngine.Runtime.RuntimeNodeType.Method, typeof(Mean)),
            new("Variance", CoreEngine.Runtime.RuntimeNodeType.Method, typeof(Variance)),
            new("Standard Deviation", CoreEngine.Runtime.RuntimeNodeType.Method, typeof(StandardDeviation)),
            new("% Return", CoreEngine.Runtime.RuntimeNodeType.Method, typeof(PercentReturn)),
            new("Correlation", CoreEngine.Runtime.RuntimeNodeType.Method, typeof(Correlation)),
            new("Covariance", CoreEngine.Runtime.RuntimeNodeType.Method, typeof(Covariance)),
            new("Covariance Matrix", CoreEngine.Runtime.RuntimeNodeType.Method, typeof(CovarianceMatrix)), // This one operates on multiple columns
            new("Min", CoreEngine.Runtime.RuntimeNodeType.Method, typeof(Min)),
            new("Max", CoreEngine.Runtime.RuntimeNodeType.Method, typeof(Max)),
            // NOT IMPLEMENTED: new("Range", CoreEngine.Runtime.RuntimeNodeType.Method, typeof(object)), // Outputs Min, Max, and Max-Min; Also displays those numbers in three lines in the node message content
            new("Sum", CoreEngine.Runtime.RuntimeNodeType.Method, typeof(Sum)),
        };
        #endregion
    }
}