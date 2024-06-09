using System;
using System.Collections.Generic;
using Parcel.Neo.Shared;
using Parcel.Neo.Shared.DataTypes;
using Parcel.Neo.Shared.Framework;
using Parcel.Neo.Shared.Framework.ViewModels;
using Parcel.Neo.Shared.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Shared.Framework.ViewModels.Primitives;

namespace Parcel.Toolbox.Present.Nodes
{
    public class Page: ProcessorNode, IWebPreviewProcessorNode
    {
        #region Node Interface
        private readonly InputConnector _serverConfigInput = new WebConfigInputConnector()
        {
            Title = "Content",
        };
        private readonly PrimitiveStringInputConnector _pageNameInput = new PrimitiveStringInputConnector()
        {
            Title = "Name",
        };
        private readonly OutputConnector _serverConfigOutput = new OutputConnector(typeof(ServerConfig))
        {
            Title = "Config",
        };
        public Page()
        {
            Title = NodeTypeName = "Page";
            Input.Add(_pageNameInput);
            Input.Add(_serverConfigInput);
            Output.Add(_serverConfigOutput);
        }
        #endregion
        
        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            ServerConfig incomeConfig = _serverConfigInput.FetchInputValue<ServerConfig>();
            string pageName = _pageNameInput.FetchInputValue<string>();
            ServerConfig newConfig = new ServerConfig()
            {
                Children = new List<ServerConfig>() {incomeConfig},
                ContentType = CacheDataType.String,
                ObjectContent = pageName,
                LayoutSpec = LayoutElementType.Page,
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
                new Tuple<ToolboxNodeExport, Vector2D, InputConnector>(new ToolboxNodeExport("String", typeof(StringNode)), new Vector2D(-150, -50), _pageNameInput),
            };
        #endregion
        
        #region Serialization
        protected override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; } =
            null;
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; } = null;
        #endregion
    }
}