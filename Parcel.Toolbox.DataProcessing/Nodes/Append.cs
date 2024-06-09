using System.Collections.Generic;
using System.Linq;
using Parcel.Neo.Shared.DataTypes;
using Parcel.Neo.Shared.Framework;
using Parcel.Neo.Shared.Framework.ViewModels;
using Parcel.Neo.Shared.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Shared.Serialization;

namespace Parcel.Toolbox.DataProcessing.Nodes
{
    public class Append: DynamicInputProcessorNode
    {
        #region Node Interface
        private readonly OutputConnector _dataTableOutput = new OutputConnector(typeof(DataGrid))
        {
            Title = "Combined Table",
        };
        public Append()
        {
            VariantInputConnectorsSerialization = new NodeSerializationRoutine(() => SerializationHelper.Serialize(Input.Count), o =>
            {
                Input.Clear();
                int count = SerializationHelper.GetInt(o);
                for (int i = 0; i < count; i++)
                    AddInputs();
            });
            
            Title = NodeTypeName = "Append";
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
            Input.Add(new InputConnector(typeof(DataGrid)){Title = $"Table {Input.Count + 1}"});
        }
        private void RemoveInputs()
        {
            Input.RemoveAt(Input.Count - 1);
        }
        #endregion
        
        #region Processor Interface
        public override OutputConnector MainOutput => _dataTableOutput as OutputConnector;

        protected override NodeExecutionResult Execute()
        {
            AppendParameter parameter = new AppendParameter()
            {
                InputTables = Input
                    .Where(i => i.Connections.Count != 0)
                    .Select(input => input.FetchInputValue<DataGrid>()).ToArray(),
            };
            DataProcessingHelper.Append(parameter);

            return new NodeExecutionResult(new NodeMessage($"{parameter.OutputTable.Columns.Count} Columns"), new Dictionary<OutputConnector, object>()
            {
                {_dataTableOutput, parameter.OutputTable}
            });
        }
        #endregion
        
        #region Serialization
        protected override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; } =
            null;
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; }
        #endregion
    }
}