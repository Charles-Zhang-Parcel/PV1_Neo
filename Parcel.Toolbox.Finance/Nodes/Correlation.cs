using System.Collections.Generic;
using Parcel.Neo.Base.DataTypes;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Toolbox.Finance.Nodes
{
    public class Correlation: ProcessorNode
    {
        #region Node Interface
        private readonly InputConnector _dataTableInput1 = new InputConnector(typeof(DataGrid))
        {
            Title = "Data Table 1",
        };
        private readonly InputConnector _columnNameInput1 = new InputConnector(typeof(string))
        {
            Title = "Column Name",
        };
        private readonly InputConnector _dataTableInput2 = new InputConnector(typeof(DataGrid))
        {
            Title = "Data Table 2",
        };
        private readonly InputConnector _columnNameInput2 = new InputConnector(typeof(string))
        {
            Title = "Column Name",
        };
        private readonly OutputConnector _valueOutput = new OutputConnector(typeof(double))
        {
            Title = "Value",
        };
        public Correlation()
        {
            Title = NodeTypeName = "Correlation";
            Input.Add(_dataTableInput1);
            Input.Add(_columnNameInput1);
            Input.Add(_dataTableInput2);
            Input.Add(_columnNameInput2);
            Output.Add(_valueOutput);
        }
        #endregion
        
        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            DataGrid dataGrid1 = _dataTableInput1.FetchInputValue<DataGrid>();
            string columnName1 = _columnNameInput1.FetchInputValue<string>();
            DataGrid dataGrid2 = _dataTableInput2.FetchInputValue<DataGrid>();
            string columnName2 = _columnNameInput2.FetchInputValue<string>();
            CorrelationParameter parameter = new CorrelationParameter()
            {
                InputTable1 = dataGrid1,
                InputColumnName1 = columnName1,
                InputTable2 = dataGrid2,
                InputColumnName2 = columnName2
            };
            FinanceHelper.Correlation(parameter);

            return new NodeExecutionResult(new NodeMessage($"Correlation={parameter.OutputValue}"), new Dictionary<OutputConnector, object>()
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