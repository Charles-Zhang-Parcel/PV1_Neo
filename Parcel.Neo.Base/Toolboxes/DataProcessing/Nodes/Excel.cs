using System.Collections.Generic;
using Parcel.Neo.Base.DataTypes;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Toolboxes.DataProcessing.Nodes
{
    public class Excel : ProcessorNode
    {
        #region Node Interface
        private readonly InputConnector _pathInput = new PrimitiveStringInputConnector()
        {
            Title = "Path",
        };
        private readonly InputConnector _headerInput = new PrimitiveBooleanInputConnector()
        {
            Title = "Contains Header"
        };
        private readonly OutputConnector _dataTableOutput = new OutputConnector(typeof(DataGrid))
        {
            Title = "Data Table"
        };
        public Excel()
        {
            Title = NodeTypeName = "Excel";
            Input.Add(_pathInput);
            Input.Add(_headerInput);
            Output.Add(_dataTableOutput);
        }
        #endregion

        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            ExcelParameter parameter = new ExcelParameter()
            {
                InputPath = _pathInput.FetchInputValue<string>(),
                InputContainsHeader = _headerInput.FetchInputValue<bool>()
            };
            DataProcessingHelper.Excel(parameter);

            return new NodeExecutionResult(new NodeMessage($"{parameter.OutputTable.RowCount} Rows; {parameter.OutputTable.ColumnCount} Columns."), new Dictionary<OutputConnector, object>()
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