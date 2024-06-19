using System.Collections.Generic;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Toolboxes.Math.Nodes
{
    public class Divide: ProcessorNode
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
        public Divide()
        {
            Title = NodeTypeName = "Divide";
            Input.Add(_number1Input);
            Input.Add(_number2Input);
            Output.Add(_resultOutput);
        }
        #endregion
        
        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            double number1 = _number1Input.FetchInputValue<double>();
            double number2 = _number2Input.FetchInputValue<double>();
            double ratio = MathHelper.Divide(number1, number2);

            return new NodeExecutionResult(new NodeMessage($"{number1}÷{number2}={ratio}"), new Dictionary<OutputConnector, object>()
            {
                { _resultOutput, ratio}
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