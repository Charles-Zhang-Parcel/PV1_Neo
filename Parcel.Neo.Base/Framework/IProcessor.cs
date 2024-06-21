using System;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.DataTypes;

namespace Parcel.Neo.Base.Framework
{
    public interface IProcessor
    {
        /// <remarks>
        /// The concept of a "MainOutput" is needed mostly because we want to provide default Preview behavior.
        /// Conventionally, we could have just taken the first output pin as main output.
        /// </remarks>
        public OutputConnector MainOutput { get; }
        public void Evaluate();
        /// <remarks>
        /// Notice each node can have multiple outputs so it's essential that we provide cache at each output level.
        /// </remarks>
        public ConnectorCache this[OutputConnector cacheConnector] { get; }
        public bool HasCache(OutputConnector cacheConnector);
    }

    /// <summary>
    /// Automatic nodes provides a way to quickly define a large library of simple function nodes without explicitly defining classes for them
    /// </summary>
    public class AutomaticNodeDescriptor
    {
        public string NodeName { get; }
        public Type[] InputTypes { get; }
        public Type[] OutputTypes { get; }
        public Func<object[], object[]> CallMarshal { get; }

        #region Additional Payload
        public string[] InputNames { get; set; }
        public string[] OutputNames { get; set; }
        #endregion

        public AutomaticNodeDescriptor(string nodeName, Type[] inputTypes, Type[] outputTypes, Func<object[], object[]> callMarshal)
        {
            NodeName = nodeName;
            InputTypes = inputTypes;
            OutputTypes = outputTypes;
            CallMarshal = callMarshal;
        }
        public AutomaticNodeDescriptor(string nodeName, Type[] inputTypes, Type outputType, Func<object[], object> callMarshal)
        {
            NodeName = nodeName;
            InputTypes = inputTypes;
            OutputTypes = [outputType];
            CallMarshal = (inputs) => new []{ callMarshal(inputs) };
        }
    }
}