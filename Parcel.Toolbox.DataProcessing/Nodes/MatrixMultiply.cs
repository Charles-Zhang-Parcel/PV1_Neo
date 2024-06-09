using System.Collections.Generic;
using System.Linq;
using Parcel.Neo.Shared.DataTypes;
using Parcel.Neo.Shared.Framework;
using Parcel.Neo.Shared.Framework.ViewModels;
using Parcel.Neo.Shared.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Shared.Serialization;

namespace Parcel.Toolbox.DataProcessing.Nodes
{
    public class MatrixMultiply: DynamicInputProcessorNode
    {
        #region Node Interface
        private readonly OutputConnector _dataTableOutput = new OutputConnector(typeof(DataGrid))
        {
            Title = "Result"
        };
        public MatrixMultiply()
        {
            VariantInputConnectorsSerialization = new NodeSerializationRoutine(() => SerializationHelper.Serialize(Input.Count), o =>
            {
                Input.Clear();
                int count = SerializationHelper.GetInt(o);
                for (int i = 0; i < count; i++)
                    AddInputs();
            });
            
            Title = NodeTypeName = "Matrix Multiply";
            Output.Add(_dataTableOutput);

            AddInputs();
            
            AddEntryCommand = new RequeryCommand(
                AddInputs,
                () => true);
            RemoveEntryCommand = new RequeryCommand(
                RemoveInputs,
                () => Input.Count > 1);
        }
        #endregion

        #region Routines
        private void AddInputs()
        {
            Input.Add(new InputConnector(typeof(DataGrid)){Title = $"Table {Input.Count / 2 + 1}"});
            Input.Add(new PrimitiveBooleanInputConnector() {Title = "Transpose"} );
        }
        private void RemoveInputs()
        {
            Input.RemoveAt(Input.Count - 1);
            Input.RemoveAt(Input.Count - 1);
        }
        #endregion

        #region View Binding/Internal Node Properties
        #endregion
        
        #region Serialization
        protected override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; } =
            null;
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; }
        #endregion

        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            MatrixMultiplyParameter parameter = new MatrixMultiplyParameter()
            {
                InputTables = Input
                    .Where((input, index) => index % 2 == 0)
                    .Select(input => input.FetchInputValue<DataGrid>()).ToArray(),
                InputTableShouldTransposes = Input
                    .Where((input, index) => index % 2 == 1)
                    .Select(input => input.FetchInputValue<bool>()).ToArray(),
            };
            DataProcessingHelper.MatrixMultiply(parameter);

            return new NodeExecutionResult(new NodeMessage($"{parameter.OutputTable.RowCount} Rows; {parameter.OutputTable.ColumnCount} Columns."), new Dictionary<OutputConnector, object>()
            {
                {_dataTableOutput, parameter.OutputTable}
            });
        }
        #endregion
    }
}