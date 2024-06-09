using System.Collections.Generic;
using Parcel.Neo.Base.DataTypes;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Toolbox.Finance.Nodes
{
    public class Covariance: ProcessorNode
    {
        #region Node Interface
        private readonly InputConnector _dataTable1Input = new InputConnector(typeof(DataGrid))
        {
            Title = "Data Table 1",
        };
        private readonly InputConnector _columnName1Input = new InputConnector(typeof(string))
        {
            Title = "Column Name",
        };
        private readonly InputConnector _dataTable2Input = new InputConnector(typeof(DataGrid))
        {
            Title = "Data Table 2",
        };
        private readonly InputConnector _columnName2Input = new InputConnector(typeof(string))
        {
            Title = "Column Name",
        };
        private readonly OutputConnector _valueOutput = new OutputConnector(typeof(double))
        {
            Title = "Value",
        };
        public Covariance()
        {
            Title = NodeTypeName = "Covariance";
            Input.Add(_dataTable1Input);
            Input.Add(_columnName1Input);
            Input.Add(_dataTable2Input);
            Input.Add(_columnName2Input);
            Output.Add(_valueOutput);
        }
        #endregion
        
        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            DataGrid dataGrid1 = _dataTable1Input.FetchInputValue<DataGrid>();
            string columnName1 = _columnName1Input.FetchInputValue<string>();
            DataGrid dataGrid2 = _dataTable2Input.FetchInputValue<DataGrid>();
            string columnName2 = _columnName2Input.FetchInputValue<string>();
            CovarianceParameter parameter = new CovarianceParameter()
            {
                InputTable1 = dataGrid1,
                InputColumnName1 = columnName1,
                InputTable2 = dataGrid2,
                InputColumnName2 = columnName2
            };
            FinanceHelper.Covariance(parameter);

            return new NodeExecutionResult(new NodeMessage($"Covariance={parameter.OutputValue}"), new Dictionary<OutputConnector, object>()
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