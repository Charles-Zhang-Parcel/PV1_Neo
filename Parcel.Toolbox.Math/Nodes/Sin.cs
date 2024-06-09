using System.Collections.Generic;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Toolbox.Math.Nodes
{
    public class Sin: ProcessorNode
    {
        #region Node Interface
        private readonly InputConnector _angleInput = new PrimitiveNumberInputConnector()
        {
            Title = "Angle",
        };
        private readonly OutputConnector _resultOutput = new OutputConnector(typeof(double))
        {
            Title = "Result",
        };
        public Sin()
        {
            Title = NodeTypeName = "Sin";
            Input.Add(_angleInput);
            Output.Add(_resultOutput);
        }
        #endregion
        
        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            double angle = _angleInput.FetchInputValue<double>();
            SinParameter parameter = new SinParameter()
            {
                InputAngle = angle,
            };
            MathHelper.Sin(parameter);

            return new NodeExecutionResult(new NodeMessage($"sin({angle})={parameter.OutputNumber}"), new Dictionary<OutputConnector, object>()
            {
                {_resultOutput, parameter.OutputNumber}
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