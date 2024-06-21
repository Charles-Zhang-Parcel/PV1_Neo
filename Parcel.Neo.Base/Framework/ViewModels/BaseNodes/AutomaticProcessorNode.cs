using System;
using System.Collections.Generic;
using System.Linq;
using Parcel.Types;
using Parcel.Neo.Base.Serialization;
using Parcel.Neo.Base.DataTypes;

namespace Parcel.Neo.Base.Framework.ViewModels.BaseNodes
{
    /// <summary>
    /// An encapsulation of a base node instance that's generated directly from methods;
    /// We will start with only a single output but there shouldn't be much difficulty outputting more outputs
    /// </summary>
    public class AutomaticProcessorNode: ProcessorNode
    {
        #region Constructor
        public AutomaticProcessorNode()
        {
            ProcessorNodeMemberSerialization = new Dictionary<string, NodeSerializationRoutine>()
            {
                {nameof(AutomaticNodeType), new NodeSerializationRoutine(() => SerializationHelper.Serialize(AutomaticNodeType), value => AutomaticNodeType = SerializationHelper.GetString(value))},
                //{nameof(InputTypes), new NodeSerializationRoutine(() => SerializationHelper.Serialize(InputTypes), value => InputTypes = SerializationHelper.GetCacheDataTypes(value))},
                //{nameof(OutputTypes), new NodeSerializationRoutine(() => SerializationHelper.Serialize(OutputTypes), value => OutputTypes = SerializationHelper.GetCacheDataTypes(value))},
                {nameof(InputNames), new NodeSerializationRoutine(() => SerializationHelper.Serialize(InputNames), value => InputNames = SerializationHelper.GetStrings(value))},
                {nameof(OutputNames), new NodeSerializationRoutine(() => SerializationHelper.Serialize(OutputNames), value => OutputNames = SerializationHelper.GetStrings(value))},
            };
        }
        private AutomaticNodeDescriptor Descriptor { get; } // Remark-cz: Hack we are saving descriptor here for easier invoking of dynamic types; However, this is not serializable at the moment! The reason we don't want it is because the descriptor itself is not serialized which means when the graph is loaded all such information is gone - and that's why we had IToolboxDefinition before.
        public AutomaticProcessorNode(AutomaticNodeDescriptor descriptor) :this()
        {
            // Remark-cz: Hack we are saving descriptor here for easier invoking of dynamic types; However, this is not serializable at the mometn!
            Descriptor = descriptor;

            // Serialization
            AutomaticNodeType = descriptor.NodeName;
            InputTypes = descriptor.InputTypes;
            OutputTypes = descriptor.OutputTypes;
            InputNames = descriptor.InputNames;
            OutputNames = descriptor.OutputNames;
            
            // Population
            PopulateInputsOutputs();
        }
        #endregion

        #region Routines
        private Func<object[], object[]> RetrieveCallMarshal()
        {
            try
            {
                if (Descriptor != null)
                {
                    // This is runtime only!
                    return Descriptor.CallMarshal;
                }
                else 
                {
                    // Remark-cz: This is more general and can handle serialization well
                    //IToolboxDefinition toolbox = (IToolboxDefinition)Activator.CreateInstance(Type.GetType(ToolboxFullName));
                    //AutomaticNodeDescriptor descriptor = toolbox.AutomaticNodes.Single(an => an != null && an.NodeName == AutomaticNodeType);
                    //return descriptor.CallMarshal;
                    throw new NotImplementedException();
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Failed to retrieve node: {e.Message}.");
            }
        }
        private void PopulateInputsOutputs()
        {
            Title = NodeTypeName = AutomaticNodeType;
            for (int index = 0; index < InputTypes.Length; index++)
            {
                Type inputType = InputTypes[index];
                string preferredTitle = InputNames?[index];
                if (inputType == typeof(bool))
                    Input.Add(new PrimitiveBooleanInputConnector() { Title = preferredTitle ?? "Bool" });
                else if (inputType == typeof(string))
                    Input.Add(new PrimitiveStringInputConnector() { Title = preferredTitle ?? "String" });
                else if (inputType == typeof(double))
                    Input.Add(new PrimitiveNumberInputConnector() { Title = preferredTitle ?? "Number" });
                else if (inputType == typeof(DateTime))
                    Input.Add(new PrimitiveDateTimeInputConnector() { Title = preferredTitle ?? "Date" });
                else 
                    Input.Add(new InputConnector(inputType) { Title = preferredTitle ?? "Input" });
            }

            for (int index = 0; index < OutputTypes.Length; index++)
            {
                Type outputType = OutputTypes[index];
                string? preferredTitle = OutputNames == null ? GetPreferredTitle(outputType) : OutputNames?[index];
                Output.Add(new OutputConnector(outputType) { Title = preferredTitle ?? "Result" });
            }

            static string? GetPreferredTitle(Type type)
            {
                if (type == typeof(bool))
                    return "Truth";
                else if (type == typeof(string))
                    return "Value";
                else if (type == typeof(double))
                    return "Number";
                else if (type == typeof(DateTime))
                    return "Date";
                else if (type == typeof(DataGrid) || type == typeof(DataColumn))
                    return "Data";
                else
                    return null;
            }
        }
        #endregion

        #region Properties
        private string AutomaticNodeType { get; set; }
        private Type[] InputTypes { get; set; }
        private Type[] OutputTypes { get; set; }
        private string[] InputNames { get; set; }
        private string[] OutputNames { get; set; }
        #endregion

        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            Dictionary<OutputConnector, object> cache = new Dictionary<OutputConnector, object>();
            
            Func<object[], object[]> marshal = RetrieveCallMarshal();
            object[] outputs = marshal.Invoke(Input.Select(i => i.FetchInputValue<object>()).ToArray());
            for (int index = 0; index < outputs.Length; index++)
            {
                object output = outputs[index];
                OutputConnector connector = Output[index];
                cache[connector] = output;
            }

            return new NodeExecutionResult(new NodeMessage(), cache);
        }
        #endregion

        #region Serialization
        protected sealed override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; }
        internal override void PostDeserialization()
        {
            base.PostDeserialization();
            PopulateInputsOutputs();
        }
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; } = null;
        #endregion

        #region Auto-Connect Interface
        public override Tuple<ToolboxNodeExport, Vector2D, InputConnector>[] AutoPopulatedConnectionNodes
        {
            get
            {
                List<Tuple<ToolboxNodeExport, Vector2D, InputConnector>> auto = [];
                for (int i = 0; i < Input.Count; i++)
                {
                    if(!InputConnectorShouldRequireAutoConnection(Input[i])) continue;

                    throw new NotImplementedException();
                    //Type nodeType = InputTypes[i];
                    //ToolboxNodeExport toolDef = new ToolboxNodeExport(Input[i].Title, nodeType);
                    //auto.Add(new Tuple<ToolboxNodeExport, Vector2D, InputConnector>(toolDef, new Vector2D(-100, -50 + (i - 1) * 50), Input[i]));
                }
                return [.. auto];
            }
        }

        public override bool ShouldHaveAutoConnection => Input.Count > 0 && Input.Any(InputConnectorShouldRequireAutoConnection);
        #endregion
    }
}