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
    public class Exclude : DynamicInputProcessorNode
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
        public Exclude()
        {
            VariantInputConnectorsSerialization = new NodeSerializationRoutine(() => SerializationHelper.Serialize(Input.Count - 1), o =>
            {
                Input.Clear();
                int count = SerializationHelper.GetInt(o);
                Input.Add(_dataTableInput);
                for (int i = 0; i < count; i++)
                    AddInputs();
            });

            Title = NodeTypeName = "Exclude";
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
            Input.Add(new PrimitiveStringInputConnector() { Title = "Column Name" });
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
            DataGrid dataGrid = _dataTableInput.FetchInputValue<DataGrid>();
            ExcludeParameter parameter = new ExcludeParameter()
            {
                InputTable = dataGrid,
                InputColumnNames = Input.Skip(1)
                    .Select(input => input.FetchInputValue<string>()).ToArray(),
            };
            DataProcessingHelper.Exclude(parameter);

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

        #region Auto Generate Interface
        public override Tuple<ToolboxNodeExport, Vector2D, InputConnector>[] AutoGenerateNodes
        {
            get
            {
                List<Tuple<ToolboxNodeExport, Vector2D, InputConnector>> auto =
                    new List<Tuple<ToolboxNodeExport, Vector2D, InputConnector>>();
                for (int i = 1; i < Input.Count; i++)
                {
                    if (Input[i].Connections.Count != 0) continue;

                    ToolboxNodeExport toolDef = new ToolboxNodeExport("Column Name", typeof(StringNode));
                    auto.Add(new Tuple<ToolboxNodeExport, Vector2D, InputConnector>(toolDef, new Vector2D(-100, -50 + (i - 1) * 50), Input[i] as InputConnector));
                }
                return auto.ToArray();
            }
        }
        public override bool ShouldHaveAutoConnection => _dataTableInput.Connections.Count == 0 ||
                                                     Input.Count > 1 && Input.Skip(1).Any(i => i.Connections.Count == 0);
        #endregion
    }
}