using System;
using System.Collections.Generic;
using System.Linq;
using Parcel.Types;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Base.Framework.ViewModels.Primitives;
using Parcel.Neo.Base.Serialization;
using Parcel.Neo.Base.DataTypes;

namespace Parcel.Neo.Base.Toolboxes.DataProcessing.Nodes
{
    public class Rename : DynamicInputProcessorNode
    {
        #region Node Interface
        private readonly InputConnector _dataTableInput = new InputConnector(typeof(DataGrid))
        {
            Title = "Original Table"
        };
        private readonly OutputConnector _dataTableOutput = new OutputConnector(typeof(DataGrid))
        {
            Title = "Data Table"
        };
        public Rename()
        {
            VariantInputConnectorsSerialization = new NodeSerializationRoutine(() => SerializationHelper.Serialize(Input.Count - 1), o =>
            {
                Input.Clear();
                int count = SerializationHelper.GetInt(o);
                Input.Add(_dataTableInput);
                for (int i = 0; i < count; i++)
                    AddInputs();
            });

            Title = NodeTypeName = "Rename";
            Input.Add(_dataTableInput);
            Output.Add(_dataTableOutput);

            AddInputs();

            AddEntryCommand = new RequeryCommand(
                AddInputs,
                () => true);
            RemoveEntryCommand = new RequeryCommand(
                RemoveInputs,
                () => Input.Count > 2);
        }
        #endregion

        #region Routines
        private void AddInputs()
        {
            Input.Add(new PrimitiveStringInputConnector() { Title = "Column" });
            Input.Add(new PrimitiveStringInputConnector() { Title = "New Name" });
        }
        private void RemoveInputs()
        {
            Input.RemoveAt(Input.Count - 1);
            Input.RemoveAt(Input.Count - 1);
        }
        #endregion

        #region View Binding/Internal Node Properties
        #endregion

        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            RenameParameter parameter = new RenameParameter()
            {
                InputTable = _dataTableInput.FetchInputValue<DataGrid>(),
                InputColumns = Input.Skip(1)
                    .Where((input, index) => index % 2 == 0)
                    .Select(input => input.FetchInputValue<string>()).ToArray(),
                InputColumnNewNames = Input.Skip(1)
                    .Where((input, index) => index % 2 == 1)
                    .Select(input => input.FetchInputValue<string>()).ToArray(),
            };
            DataProcessingHelper.Rename(parameter);

            return new NodeExecutionResult(new NodeMessage($"{(Input.Count - 1) / 2} columns renamed."), new Dictionary<OutputConnector, object>()
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

        #region Auto-Connect Interface
        public override Tuple<ToolboxNodeExport, Vector2D, InputConnector>[] AutoGenerateNodes
        {
            get
            {
                List<Tuple<ToolboxNodeExport, Vector2D, InputConnector>> auto =
                    new List<Tuple<ToolboxNodeExport, Vector2D, InputConnector>>();
                for (int i = 1; i < Input.Count; i += 2)
                {
                    if (!InputConnectorShouldRequireAutoConnection(Input[i])) continue;

                    ToolboxNodeExport toolDef = new ToolboxNodeExport("Input Name", typeof(StringNode));
                    auto.Add(new Tuple<ToolboxNodeExport, Vector2D, InputConnector>(toolDef, new Vector2D(-100, -50 + (i - 1) * 50), Input[i] as InputConnector));
                    auto.Add(new Tuple<ToolboxNodeExport, Vector2D, InputConnector>(toolDef, new Vector2D(-100, (i - 1) * 50), Input[i + 1] as InputConnector));
                }
                return auto.ToArray();
            }
        }
        public override bool ShouldHaveAutoConnection => Input.Count > 1 && Input.Skip(1).Any(InputConnectorShouldRequireAutoConnection);
        #endregion
    }
}