using System.Collections.Generic;
using Parcel.Neo.Base.DataTypes;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Toolboxes.Finance.Nodes
{
    public class Min: ProcessorNode
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
        public Min()
        {
            Title = NodeTypeName = "Min";
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
            MinParameter parameter = new MinParameter()
            {
                InputTable = dataGrid,
                InputColumnName = columnName
            };
            FinanceHelper.Min(parameter);

            return new NodeExecutionResult(new NodeMessage($"Min={parameter.OutputValue}"), new Dictionary<OutputConnector, object>()
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