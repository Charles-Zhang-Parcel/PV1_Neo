using System.Collections.Generic;
using Parcel.Neo.Base.DataTypes;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Toolbox.Finance.Nodes
{
    /// <remarks>
    /// If wanted, the option for population vs sample (n-1) vs n choice can be exposed as property inside property window,
    /// instead of exposing as pins. To avoid clustering the view. 
    /// </remarks>
    public class CovarianceMatrix: ProcessorNode
    {
        #region Node Interface
        private readonly InputConnector _dataTableInput = new InputConnector(typeof(DataGrid))
        {
            Title = "Data Table",
        };
        private readonly OutputConnector _dataTableOutput = new OutputConnector(typeof(DataGrid))
        {
            Title = "Result",
        };
        public CovarianceMatrix()
        {
            Title = NodeTypeName = "Covariance Matrix";
            Input.Add(_dataTableInput);
            Output.Add(_dataTableOutput);
        }
        #endregion
        
        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            DataGrid dataGrid = _dataTableInput.FetchInputValue<DataGrid>();
            CovarianceMatrixParameter parameter = new CovarianceMatrixParameter()
            {
                InputTable = dataGrid
            };
            FinanceHelper.CovarianceMatrix(parameter);

            return new NodeExecutionResult(new NodeMessage($"{parameter.OutputTable.Columns.Count} Rows {parameter.OutputTable.Columns[0].Length} Columns"), new Dictionary<OutputConnector, object>()
            {
                {_dataTableOutput, parameter.OutputTable}
            });
        }
        #endregion
        
        #region Serialization
        protected override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; } =
            null;
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; } = null;
        #endregion
    }
}