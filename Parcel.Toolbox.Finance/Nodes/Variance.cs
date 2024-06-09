using System.Collections.Generic;
using Parcel.Neo.Shared.DataTypes;
using Parcel.Neo.Shared.Framework;
using Parcel.Neo.Shared.Framework.ViewModels;
using Parcel.Neo.Shared.Framework.ViewModels.BaseNodes;

namespace Parcel.Toolbox.Finance.Nodes
{
    public class Variance: ProcessorNode
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
        public Variance()
        {
            Title = NodeTypeName = "Variance";
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
            VarianceParameter parameter = new VarianceParameter()
            {
                InputTable = dataGrid,
                InputColumnName = columnName
            };
            FinanceHelper.Variance(parameter);

            return new NodeExecutionResult(new NodeMessage($"Variance={parameter.OutputValue}"), new Dictionary<OutputConnector, object>()
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