using System.Collections.Generic;
using Parcel.Types;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using Parcel.Types;

namespace Parcel.Neo.Base.Toolboxes.Finance.Nodes
{
    public class PercentReturn: ProcessorNode
    {
        #region Node Interface
        private readonly InputConnector _dataTableInput = new(typeof(DataGrid))
        {
            Title = "Data Table",
        };

        private readonly InputConnector _latestAtTopInput = new PrimitiveBooleanInputConnector()
        {
            Title = "Latest At Top"
        };
        private readonly OutputConnector _dataTableOutput = new(typeof(DataGrid))
        {
            Title = "Result",
        };
        public PercentReturn()
        {
            Title = NodeTypeName = "PercentReturn";
            Input.Add(_dataTableInput);
            Input.Add(_latestAtTopInput);
            Output.Add(_dataTableOutput);
            
            Tooltip = $"This node takes in a table of time series data in each column; It will automatically trim rows; Non-numerical data will be trimmed.";
            Message.Content = "Input a time series.";
            Message.Type = NodeMessageType.Documentation;
        }
        #endregion
        
        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            DataGrid dataGrid = _dataTableInput.FetchInputValue<DataGrid>();
            bool latestAtTop = _latestAtTopInput.FetchInputValue<bool>();
            PercentReturnParameter parameter = new()
            {
                InputTable = dataGrid,
                LatestAtTop = latestAtTop
            };
            FinanceHelper.PercentReturn(parameter);

            return new NodeExecutionResult(new NodeMessage($"{parameter.OutputTable.RowCount} Rows, {parameter.OutputTable.ColumnCount} Columns"), new Dictionary<OutputConnector, object>()
            {
                {_dataTableOutput, parameter.OutputTable}
            });
        }
        #endregion
        
        #region Serialization
        protected override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; } = null;
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; } = null;
        #endregion
    }
}