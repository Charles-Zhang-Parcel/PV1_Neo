using System;
using System.Collections.Generic;
using System.Linq;
using Parcel.Neo.Base.DataTypes;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Base.Serialization;

namespace Parcel.Toolbox.Logic.Nodes
{
    public class Choose: DynamicInputProcessorNode
    {
        #region Node Interface
        private readonly InputConnector _selectorInput = new PrimitiveNumberInputConnector()
        {
            Title = "Selector",
        };
        private readonly OutputConnector _dataTableOutput = new OutputConnector(typeof(DataGrid))
        {
            Title = "Selected Table",
        };
        public Choose()
        {
            VariantInputConnectorsSerialization = new NodeSerializationRoutine(() => SerializationHelper.Serialize(Input.Count - 1), o =>
            {
                Input.Clear();
                Input.Add(_selectorInput);
                int count = SerializationHelper.GetInt(o);
                for (int i = 0; i < count; i++)
                    AddInputs();
            });
            
            Title = NodeTypeName = "Choose";
            Input.Add(_selectorInput);
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
            Input.Add(new InputConnector(typeof(DataGrid)){Title = $"Table {Input.Count}"});
        }
        private void RemoveInputs()
        {
            Input.RemoveAt(Input.Count - 1);
        }
        #endregion
        
        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            int selector = (int)Input[0].FetchInputValue<double>();
            DataGrid[] inputTables = Input.Skip(1)
                .Where(i => i.Connections.Count != 0)
                .Select(input => input.FetchInputValue<DataGrid>()).ToArray();

            if (selector < 1 || selector > inputTables.Length)
                throw new ArgumentException("Invalid selector.");

            return new NodeExecutionResult(new NodeMessage($"Choose Table {selector}"), new Dictionary<OutputConnector, object>()
            {
                {_dataTableOutput, inputTables[selector - 1]}
            });
        }
        #endregion
        
        #region Serialization

        protected sealed override Dictionary<string, NodeSerializationRoutine>
            ProcessorNodeMemberSerialization { get; } = null;
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; }

        #endregion
    }
}