using System.Collections.Generic;
using Parcel.Neo.Base.DataTypes;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Toolboxes.Finance.Nodes
{
    public class Max: ProcessorNode
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
        private readonly OutputConnector _valueOutput = new OutputConnector(typeof(double))
        {
            Title = "Value",
        };
        public Max()
        {
            Title = NodeTypeName = "Max";
            Input.Add(_dataTableInput);
            Input.Add(_columnNameInput);
            Output.Add(_valueOutput);
        }
        #endregion
        
        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            DataGrid dataGrid = _dataTableInput.FetchInputValue<DataGrid>();
            string columnName = _columnNameInput.FetchInputValue<string>();
            MaxParameter parameter = new MaxParameter()
            {
                InputTable = dataGrid,
                InputColumnName = columnName
            };
            FinanceHelper.Max(parameter);

            return new NodeExecutionResult(new NodeMessage($"Max={parameter.OutputValue}"), new Dictionary<OutputConnector, object>()
            {
                {_valueOutput, parameter.OutputValue}
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