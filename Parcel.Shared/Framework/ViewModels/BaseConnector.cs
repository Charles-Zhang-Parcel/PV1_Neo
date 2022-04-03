﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Parcel.Shared.DataTypes;
using Parcel.Shared.Framework.ViewModels.BaseNodes;

namespace Parcel.Shared.Framework.ViewModels
{
    public enum ConnectorFlowType
    {
        Input,
        Output
    }

    public enum ConnectorShape
    {
        Circle, // Default; Primitive (string and number)
        Triangle, // Compound data
        Square, // Boolean
    }

    
    public class BaseConnector: ObservableObject
    {
        #region View Properties
        private string? _title;
        public string? Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }
        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set => SetField(ref _isConnected, value);
        }
        private bool _isHidden;
        public bool IsHidden
        {
            get => _isHidden;
            set => SetField(ref _isHidden, value);
        }
        private Point _anchor;
        public Point Anchor
        {
            get => _anchor;
            set => SetField(ref _anchor, value);
        }
        private BaseNode _node = default!;
        public BaseNode Node
        {
            get => _node;
            internal set
            {
                if (SetField(ref _node, value))
                {
                    OnNodeChanged();
                }
            }
        }
        private ConnectorShape _shape;
        public ConnectorShape Shape
        {
            get => _shape;
            set => SetField(ref _shape, value);
        }
        #endregion

        #region Other Properties
        public ConnectorFlowType FlowType { get; private set; }
        public int MaxConnections { get; set; } = 2;
        public NotifyObservableCollection<BaseConnection> Connections { get; } = new NotifyObservableCollection<BaseConnection>();
        
        public Type DataType { get; set; }
        /// <summary>
        /// Used for input nodes that haven't had any input yet
        /// </summary>
        public object DefaultDataStorage { get; set; }
        #endregion

        #region Node Framework
        public BaseConnector(Type dataType)
        {
            DataType = dataType;
            Shape = DecideShape(dataType);
            
            Connections.WhenAdded(c =>
            {
                c.Input.IsConnected = true;
                c.Output.IsConnected = true;
            }).WhenRemoved(c =>
            {
                if (c.Input.Connections.Count == 0)
                {
                    c.Input.IsConnected = false;
                }

                if (c.Output.Connections.Count == 0)
                {
                    c.Output.IsConnected = false;
                }
            });
        }
        protected virtual void OnNodeChanged()
        {
            if (Node is ProcessorNode processorNode)
            {
                FlowType = processorNode.Input.Contains(this) ? ConnectorFlowType.Input : ConnectorFlowType.Output;
            }
            else if (Node is KnotNode knotNode)
            {
                FlowType = knotNode.Flow;
            }
        }
        public bool IsConnectedTo(BaseConnector connector)
            => Connections.Any(c => c.Input == connector || c.Output == connector);
        public virtual bool AllowsNewConnections()
            => Connections.Count < MaxConnections;
        public void Disconnect()
            => Node.Graph.Schema.DisconnectConnector(this);
        #endregion

        #region Interface
        public T FetchInputValue<T>()
        {
            if (FlowType != ConnectorFlowType.Input)
                throw new InvalidOperationException("Can't fetch value for output connector.");
            if (Connections.Count > 1)
                throw new InvalidOperationException("Input connector has more than 1 connection.");

            BaseConnection connection = Connections.SingleOrDefault();
            if (typeof(T) == DataType)
            {
                if (connection != null)
                {
                    if (connection.Input.Node is KnotNode search)
                    {
                        BaseNode prev = search;
                    
                        while (prev is KnotNode knot)
                            prev = knot.Previous;

                        if(prev is ProcessorNode processor)
                            return (T)processor.ProcessorCache[connection.Input].DataObject;

                        throw new InvalidOperationException("Knot nodes connect to empty source.");
                    }
                    else if (connection.Input.Node is ProcessorNode processor)
                    {
                        return (T) processor.ProcessorCache[connection.Input].DataObject;
                    }
                    else throw new InvalidOperationException("Invalid node type.");
                }
                else
                {
                    return (T)DefaultDataStorage ?? default(T);
                }
            }
            else throw new ArgumentException("Wrong type.");
        }
        #endregion

        #region Routines
        private readonly Dictionary<Type, ConnectorShape> _mappings = new Dictionary<Type, ConnectorShape>()
        {
            {typeof(bool), ConnectorShape.Square},
            {typeof(string), ConnectorShape.Circle},
            {typeof(double), ConnectorShape.Circle},
            {typeof(DataGrid), ConnectorShape.Triangle},
        };
        private ConnectorShape DecideShape(Type dataType)
        {
            if (_mappings.ContainsKey(dataType))
                return _mappings[dataType];
            return ConnectorShape.Circle;
        }
        #endregion
    }
}