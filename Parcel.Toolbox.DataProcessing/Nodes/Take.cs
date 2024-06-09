using System.Collections.Generic;
using Parcel.Neo.Base.DataTypes;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Toolbox.DataProcessing.Nodes
{
    public class Take: ProcessorNode
    {
        #region Node Interface
        private readonly InputConnector _dataTableInput = new InputConnector(typeof(DataGrid))
        {
            Title = "Data Table",
        };
        private readonly InputConnector _rowCountInput = new InputConnector(typeof(double))
        {
            Title = "Row Count",
        };
        private readonly OutputConnector _dataTableOutput = new OutputConnector(typeof(DataGrid))
        {
            Title = "Data Table Column",
        };
        public Take()
        {
            Title = NodeTypeName = "Take";
            Input.Add(_dataTableInput);
            Input.Add(_rowCountInput);
            Output.Add(_dataTableOutput);
        }
        #endregion
        
        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            DataGrid dataGrid = _dataTableInput.FetchInputValue<DataGrid>();
            double rowCount = _rowCountInput.FetchInputValue<double>();
            TakeParameter parameter = new TakeParameter()
            {
                InputTable = dataGrid,
                InputRowCount = (int)rowCount
            };
            DataProcessingHelper.Take(parameter);

            return new NodeExecutionResult(new NodeMessage($"{parameter.OutputTable.Rows.Count} Rows"), new Dictionary<OutputConnector, object>()
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