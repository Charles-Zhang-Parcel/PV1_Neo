using System.Reflection;
using Parcel.Neo.Base.Framework;
using Parcel.Toolbox.Finance.Nodes;

namespace Parcel.Toolbox.Finance
{
    public class ToolboxDefinition: IToolboxDefinition
    {
        #region Interface
        public string ToolboxName => "Finance";
        public string ToolboxAssemblyFullName => Assembly.GetExecutingAssembly().FullName;
        public ToolboxNodeExport[] ExportNodes => new ToolboxNodeExport[] 
        {
            // Basic - Operations on Columns (Those will check and validate column type as Number/Double)
            new ToolboxNodeExport("Mean", typeof(Mean)),
            new ToolboxNodeExport("Variance", typeof(Variance)),
            new ToolboxNodeExport("Standard Deviation", typeof(StandardDeviation)),
            new ToolboxNodeExport("% Return", typeof(PercentReturn)),
            new ToolboxNodeExport("Correlation", typeof(Correlation)),
            new ToolboxNodeExport("Covariance", typeof(Covariance)),
            new ToolboxNodeExport("Covariance Matrix", typeof(CovarianceMatrix)), // This one operates on multiple columns
            new ToolboxNodeExport("Min", typeof(Min)),
            new ToolboxNodeExport("Max", typeof(Max)),
            new ToolboxNodeExport("Range", typeof(object)), // Outputs Min, Max, and Max-Min; Also displays those numbers in three lines in the node message content
            new ToolboxNodeExport("Sum", typeof(Sum)),
        };
        public AutomaticNodeDescriptor[] AutomaticNodes => new AutomaticNodeDescriptor[] { };
        #endregion
    }
}