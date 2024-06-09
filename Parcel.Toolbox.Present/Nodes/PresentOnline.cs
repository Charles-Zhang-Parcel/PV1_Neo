using System.Collections.Generic;
using System.Linq;
using Parcel.Neo.Shared;
using Parcel.Neo.Shared.DataTypes;
using Parcel.Neo.Shared.Framework;
using Parcel.Neo.Shared.Framework.ViewModels;
using Parcel.Neo.Shared.Framework.ViewModels.BaseNodes;

namespace Parcel.Toolbox.Present.Nodes
{
    public class PresentOnline: DynamicInputProcessorNode, IWebPreviewProcessorNode
    {
        #region Node Interface
        private readonly PrimitiveStringInputConnector _presentNameInput = new PrimitiveStringInputConnector()
        {
            Title = "Name",
        };
        public PresentOnline()
        {
            Title = NodeTypeName = "Present";
            Input.Add(_presentNameInput);
            
            AddInputs();
            
            AddEntryCommand = new RequeryCommand(
                () => AddInputs(),
                () => true);
            RemoveEntryCommand = new RequeryCommand(
                () => RemoveInputs(),
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
            ServerConfig config = new ServerConfig()
            {
                Children = Input.Skip(1).Select(i => i.FetchInputValue<ServerConfig>()).ToList(),
                LayoutSpec = LayoutElementType.Presentation
            };
            WebHostRuntime.Singleton.CurrentLayout = config;
            
            ((IWebPreviewProcessorNode)this).OpenWebPreview("Present");
            return new NodeExecutionResult(new NodeMessage($"Presenting..."), null);
        }
        #endregion
        
        #region Serialization
        protected override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; } =
            null;
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; } = null;
        #endregion
    }
}