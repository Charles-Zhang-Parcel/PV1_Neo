using System.Collections.Generic;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Base.Serialization;

namespace Parcel.Toolbox.Math.Nodes
{
    public class Multiply: ProcessorNode
    {
        #region Node Interface
        private readonly InputConnector _number1Input = new PrimitiveNumberInputConnector()
        {
            Title = "Number 1",
        };
        private readonly InputConnector _number2Input = new PrimitiveNumberInputConnector()
        {
            Title = "Number 2",
        };
        private readonly OutputConnector _resultOutput = new OutputConnector(typeof(double))
        {
            Title = "Result",
        };
        public Multiply()
        {
            ProcessorNodeMemberSerialization = new Dictionary<string, NodeSerializationRoutine>()
            {
                {
                    nameof(_number1Input),
                    new NodeSerializationRoutine(() => SerializationHelper.Serialize((double)_number1Input.DefaultDataStorage),
                        o => _number1Input.DefaultDataStorage = o)
                },
                {
                    nameof(_number2Input),
                    new NodeSerializationRoutine(() => SerializationHelper.Serialize((double)_number2Input.DefaultDataStorage),
                        o => _number2Input.DefaultDataStorage = o)
                },
            };
            
            Title = NodeTypeName = "Multiply";
            Input.Add(_number1Input);
            Input.Add(_number2Input);
            Output.Add(_resultOutput);
        }
        #endregion
        
        #region Processor Interface
        public override OutputConnector MainOutput => _resultOutput as OutputConnector;

        protected override NodeExecutionResult Execute()
        {
            double number1 = _number1Input.FetchInputValue<double>();
            double number2 = _number2Input.FetchInputValue<double>();
            MultiplyParameter parameter = new MultiplyParameter()
            {
                InputNumber1 = number1,
                InputNumber2 = number2
            };
            MathHelper.Multiply(parameter);

            return new NodeExecutionResult(new NodeMessage($"{number1}×{number2}={parameter.OutputNumber}"), new Dictionary<OutputConnector, object>()
            {
                {_resultOutput, parameter.OutputNumber}
            });
        }
        #endregion
        
        #region Serialization
        protected override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; }
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; } = null;
        #endregion
    }
}