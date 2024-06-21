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
            new("Mean", typeof(Mean)),
            new("Variance", typeof(Variance)),
            new("Standard Deviation", typeof(StandardDeviation)),
            new("% Return", typeof(PercentReturn)),
            new("Correlation", typeof(Correlation)),
            new("Covariance", typeof(Covariance)),
            new("Covariance Matrix", typeof(CovarianceMatrix)), // This one operates on multiple columns
            new("Min", typeof(Min)),
            new("Max", typeof(Max)),
            // NOT IMPLEMENTED: new("Range", typeof(object)), // Outputs Min, Max, and Max-Min; Also displays those numbers in three lines in the node message content
            new("Sum", typeof(Sum)),
        };
        #endregion
    }
}