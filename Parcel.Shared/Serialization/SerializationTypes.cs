﻿using System;
using System.Collections.Generic;
using Parcel.Shared.Framework.ViewModels.BaseNodes;

namespace Parcel.Shared.Serialization
{
    [Serializable]
    internal class NodeGraph
    {
        #region Graph Metadata
        public string Version { get; set; }
        
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        
        public DateTime CreationTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public int Revision { get; set; }
        #endregion
        
        #region Nodes Data
        public List<NodeData> Nodes { get; set; }
        public List<ConnectionData> Connections { get; set; }
        #endregion
    }
    
    [Serializable]
    internal class NodeData
    {
        public Type NodeType { get; set; }
        public Dictionary<string, object> NodeMembers { get; set; }

        public BaseNode Deserialize()
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    internal class ConnectionData
    {
        public NodeData Source { get; set; }
        public int SourcePin { get; set; }
        public NodeData Destination { get; set; }
        public int DestinationPin { get; set; }
    }
}