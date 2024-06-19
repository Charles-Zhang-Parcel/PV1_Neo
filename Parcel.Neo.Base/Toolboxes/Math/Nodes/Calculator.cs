using System.Collections.Generic;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Base.Serialization;

namespace Parcel.Neo.Base.Toolboxes.Math.Nodes
{
    public class Calculator: ProcessorNode
    {
        #region Node Interface
        protected readonly OutputConnector _resultOutput = new OutputConnector(typeof(double))
        {
            Title = "Result",
        };
        public Calculator()
        {
            ProcessorNodeMemberSerialization = new Dictionary<string, NodeSerializationRoutine>()
            {
                {nameof(Value), new NodeSerializationRoutine( () => SerializationHelper.Serialize(_value), value => _value = SerializationHelper.GetString(value))}
            };
            
            Title = NodeTypeName = "Calculator";
            Output.Add(_resultOutput);
        }
        #endregion
        
        #region Public View Properties
        private string _value;
        public string Value
        {
            get => _value;
            set => SetField(ref _value, value);
        }
        #endregion
        
        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            object result = new CodingSeb.ExpressionEvaluator.ExpressionEvaluator().Evaluate(Value);
            
            return new NodeExecutionResult(new NodeMessage($"{result}"), new Dictionary<OutputConnector, object>()
            {
                {_resultOutput, result}
            });
        }
        #endregion
        
        #region Serialization
        protected override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; }
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; } = null;
        #endregion
    }
}