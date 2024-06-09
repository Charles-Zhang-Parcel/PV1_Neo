using System;
using System.Collections.Generic;
using System.Linq;
using Parcel.Neo.Shared;
using Parcel.Neo.Shared.DataTypes;
using Parcel.Neo.Shared.Framework;
using Parcel.Neo.Shared.Framework.ViewModels;
using Parcel.Neo.Shared.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Shared.Framework.ViewModels.Primitives;
using Parcel.Neo.Shared.Serialization;

namespace Parcel.Toolbox.Present.Nodes
{
    public class Section: DynamicInputProcessorNode, IWebPreviewProcessorNode
    {
        #region Node Interface
        private readonly PrimitiveStringInputConnector _sectionNameInput = new PrimitiveStringInputConnector()
        {
            Title = "Name",
        };
        private readonly OutputConnector _serverConfigOutput = new OutputConnector(typeof(ServerConfig))
        {
            Title = "Config",
        };
        public Section()
        {
            VariantInputConnectorsSerialization = new NodeSerializationRoutine(() => SerializationHelper.Serialize(Input.Count), o =>
            {
                Input.Clear();
                int count = SerializationHelper.GetInt(o);
                for (int i = 0; i < count; i++)
                    AddInputs();
            });
            
            Title = NodeTypeName = "Section";
            Input.Add(_sectionNameInput);
            Output.Add(_serverConfigOutput);
            
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
            Input.Add(new WebConfigInputConnector(){ Title = "Content" });
        }
        private void RemoveInputs()
        {
            Input.RemoveAt(Input.Count - 1);
        }
        #endregion
        
        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            string sectionName = _sectionNameInput.FetchInputValue<string>();
            ServerConfig newConfig = new ServerConfig()
            {
                Children = Input.Skip(1).Select(i => i.FetchInputValue<ServerConfig>()).ToList(),
                ContentType = CacheDataType.String,
                ObjectContent = sectionName,
                LayoutSpec = LayoutElementType.Section,
            };
            WebHostRuntime.Singleton.CurrentLayout = newConfig;
            
            ((IWebPreviewProcessorNode)this).OpenWebPreview("Present");
            return new NodeExecutionResult(new NodeMessage($"Presenting..."), new Dictionary<OutputConnector, object>()
            {
                {_serverConfigOutput, newConfig}
            });
        }
        #endregion

        #region Auto-Connect Interface
        public override Tuple<ToolboxNodeExport, Vector2D, InputConnector>[] AutoGenerateNodes =>
            new Tuple<ToolboxNodeExport, Vector2D, InputConnector>[]
            {
                new Tuple<ToolboxNodeExport, Vector2D, InputConnector>(new ToolboxNodeExport("String", typeof(StringNode)), new Vector2D(-150, -50), _sectionNameInput),
            };
        #endregion
        
        #region Serialization
        protected override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; } =
            null;
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; }
        #endregion
    }
}