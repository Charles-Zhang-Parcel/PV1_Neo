using System;
using System.Collections.Generic;
using System.Linq;
using Parcel.Neo.Shared.DataTypes;
using Parcel.Neo.Shared.Framework;
using Parcel.Neo.Shared.Framework.ViewModels;
using Parcel.Neo.Shared.Framework.ViewModels.BaseNodes;

namespace Parcel.Toolbox.DataProcessing.Nodes
{
    public class Transpose: ProcessorNode
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
        public Transpose()
        {
            Title = NodeTypeName = "Transpose";
            Input.Add(_dataTableInput);
            Output.Add(_dataTableOutput);
        }
        #endregion
        
        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            DataGrid dataGrid = _dataTableInput.FetchInputValue<DataGrid>();
            TransposeParameter parameter = new TransposeParameter()
            {
                InputTable = dataGrid
            };
            DataProcessingHelper.Transpose(parameter);

            return new NodeExecutionResult(new NodeMessage($"{parameter.OutputTable.RowCount} Rows; {parameter.OutputTable.ColumnCount} Columns"), new Dictionary<OutputConnector, object>()
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