using System;
using System.Collections.Generic;
using System.Reflection;
using Parcel.Neo.Base.DataTypes;
using Parcel.Neo.Base.Framework.ViewModels;

namespace Parcel.Neo.Base.Framework
{
    public interface IProcessor
    {
        public OutputConnector MainOutput { get; }
        public void Evaluate();
        public ConnectorCacheDescriptor this[OutputConnector cacheConnector] { get; }
        public bool HasCache(OutputConnector cacheConnector);
    }

    /// <summary>
    /// Automatic nodes provides a way to quickly define a large library of simple function nodes without defining classes for them
    /// </summary>
    public class AutomaticNodeDescriptor
    {
        public string NodeName { get; }
        public CacheDataType[] InputTypes { get; }
        public CacheDataType[] OutputTypes { get; }
        public Func<object[], object[]> CallMarshal { get; }

        #region Additional Payload
        public string[] InputNames { get; set; }
        public string[] OutputNames { get; set; }
        #endregion

        public AutomaticNodeDescriptor(string nodeName, CacheDataType[] inputTypes, CacheDataType[] outputTypes, Func<object[], object[]> callMarshal)
        {
            NodeName = nodeName;
            InputTypes = inputTypes;
            OutputTypes = outputTypes;
            CallMarshal = callMarshal;
        }
        public AutomaticNodeDescriptor(string nodeName, CacheDataType[] inputTypes, CacheDataType outputType, Func<object[], object> callMarshal)
        {
            NodeName = nodeName;
            InputTypes = inputTypes;
            OutputTypes = new []{ outputType };
            CallMarshal = (inputs) => new []{ callMarshal(inputs) };
        }
    }
}