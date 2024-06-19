using System.Collections.Generic;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Toolboxes.Math.Nodes
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
            double sin = MathHelper.Sin(angle);

            return new NodeExecutionResult(new NodeMessage($"sin({angle})={sin}"), new Dictionary<OutputConnector, object>()
            {
                {_resultOutput, sin}
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