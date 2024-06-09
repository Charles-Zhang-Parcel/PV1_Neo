using System.Collections.Generic;
using Parcel.Neo.Base.DataTypes;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Toolbox.DataProcessing.Nodes
{
    public class Sort: ProcessorNode
    {
        #region Node Interface
        private readonly InputConnector _dataTableInput = new InputConnector(typeof(DataGrid))
        {
            Title = "Data Table",
        };
        private readonly InputConnector _columnNameInput = new InputConnector(typeof(string))
        {
            Title = "Column Name",
        };
        private readonly InputConnector _reverseOrderInput = new InputConnector(typeof(bool))
        {
            Title = "Reverse Order",
        };
        private readonly OutputConnector _dataTableOutput = new OutputConnector(typeof(DataGrid))
        {
            Title = "Result",
        };
        public Sort()
        {
            Title = NodeTypeName = "Sort";
            Input.Add(_dataTableInput);
            Input.Add(_columnNameInput);
            Input.Add(_reverseOrderInput);
            Output.Add(_dataTableOutput);
        }
        #endregion
        
        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            DataGrid dataGrid = _dataTableInput.FetchInputValue<DataGrid>();
            string columnName = _columnNameInput.FetchInputValue<string>();
            bool reverseOrder = _reverseOrderInput.FetchInputValue<bool>();
            SortParameter parameter = new SortParameter()
            {
                InputTable = dataGrid,
                InputColumnName = columnName,
                InputReverseOrder = reverseOrder
            };
            DataProcessingHelper.Sort(parameter);

            return new NodeExecutionResult(new NodeMessage($"{parameter.OutputTable.RowCount} Rows {parameter.OutputTable.ColumnCount} Columns"), new Dictionary<OutputConnector, object>()
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