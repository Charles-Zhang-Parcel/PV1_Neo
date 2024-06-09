using System.Collections.Generic;
using Parcel.Neo.Shared.DataTypes;
using Parcel.Neo.Shared.Framework;
using Parcel.Neo.Shared.Framework.ViewModels;
using Parcel.Neo.Shared.Framework.ViewModels.BaseNodes;

namespace Parcel.Toolbox.Basic.Nodes
{
    public class Preview: ProcessorNode
    {
        #region Node Interface
        private readonly InputConnector _objectInput = new InputConnector(typeof(object))
        {
            Title = "Object",
        };
        private readonly OutputConnector _objectOutput = new OutputConnector(typeof(object))
        {
            Title = "Object",
        };
        public Preview()
        {
            Title = NodeTypeName = "Preview";
            Input.Add(_objectInput);
            Output.Add(_objectOutput);
        }
        #endregion
        
        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            object obj = _objectInput.FetchInputValue<object>();

            return new NodeExecutionResult(new NodeMessage(obj.ToString()), new Dictionary<OutputConnector, object>()
            {
                {_objectOutput, new ConnectorCacheDescriptor(obj, CacheDataType.Generic)}
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