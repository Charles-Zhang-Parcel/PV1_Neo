﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Parcel.Shared.DataTypes;
using Parcel.Shared.Framework.ViewModels.BaseNodes;

namespace Parcel.Shared.Framework.ViewModels
{
    public class PrimitiveBooleanInputConnector : PrimitiveInputConnector
    {
        public PrimitiveBooleanInputConnector() : base(typeof(bool))
        {
            Value = false;
        }
    }
    
    public class PrimitiveDateTimeInputConnector : PrimitiveInputConnector
    {
        public PrimitiveDateTimeInputConnector() : base(typeof(DateTime))
        {
            Value = DateTime.Now.Date;
        }
    }
    
    public class PrimitiveStringInputConnector : PrimitiveInputConnector
    {
        public PrimitiveStringInputConnector() : base(typeof(string))
        {
            Value = string.Empty;
        }
    }
    
    public class PrimitiveNumberInputConnector : PrimitiveInputConnector
    {
        public PrimitiveNumberInputConnector() : base(typeof(double))
        {
            Value = 0;
        }
    }
    
    public class PrimitiveInputConnector : InputConnector
    {
        public object Value
        {
            get => _defaultDataStorage;
            set => SetField(ref _defaultDataStorage, value);
        }
        
        public PrimitiveInputConnector(Type dataType) : base(dataType)
        {
        }
    }

    public class WebConfigInputConnector : InputConnector
    {
        public WebConfigInputConnector() : base(typeof(ServerConfig))
        {
        }
    }

    public class InputConnector : BaseConnector
    {
        public InputConnector(Type dataType) : base(dataType)
        {
            FlowType = ConnectorFlowType.Input;
        }
    }

    public class OutputConnector : BaseConnector
    {
        public OutputConnector(Type dataType) : base(dataType)
        {
            FlowType = ConnectorFlowType.Output;
        }
    }
    
    public class KnotConnector : BaseConnector
    {
        public KnotConnector(Type dataType) : base(dataType)
        {
            FlowType = ConnectorFlowType.Knot;
        }
    }
    
    public abstract class BaseConnector: ObservableObject
    {
        #region View Properties
        private string _title;
        public string Title
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
        public ConnectorFlowType FlowType { get; protected set; }
        public int MaxConnections { get; set; } = int.MaxValue;
        public NotifyObservableCollection<BaseConnection> Connections { get; } = new NotifyObservableCollection<BaseConnection>();
        
        public Type DataType { get; set; }
        /// <summary>
        /// Used for input nodes that haven't had any input yet
        /// </summary>
        public object DefaultDataStorage 
        { 
            get => _defaultDataStorage;
            set => SetField(ref _defaultDataStorage, value);
        }
        protected object _defaultDataStorage;
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
            => (FlowType != ConnectorFlowType.Input && Connections.Count < MaxConnections)
                || (FlowType == ConnectorFlowType.Input && Connections.Count == 0);
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
            if (typeof(T) == DataType || typeof(T).IsAssignableFrom(DataType))
            {
                if (connection != null)
                {
                    if (connection.Input.Node is KnotNode search)
                    {
                        BaseNode prev = search;
                    
                        while (prev is KnotNode knot)
                            prev = knot.Previous;

                        if (prev is ProcessorNode processor)
                            return (T)processor[connection.Input as OutputConnector].DataObject;

                        throw new InvalidOperationException("Knot nodes connect to empty source.");
                    }
                    else if (connection.Input.Node is ProcessorNode processor)
                    {
                        return (T) processor[connection.Input as OutputConnector].DataObject;
                    }
                    else throw new InvalidOperationException("Invalid node type.");
                }
                else
                {
                    if (this is OutputConnector _outputConnector && Node is ProcessorNode processor && processor.HasCache(_outputConnector))
                        return (T) processor[_outputConnector].DataObject;
                    else
                        return DefaultDataStorage != null ? (T) DefaultDataStorage : default(T);
                }
            }
            else throw new ArgumentException("Wrong type.");
        }
        #endregion

        #region Routines
        private readonly Dictionary<Type, ConnectorShape> _mappings = new Dictionary<Type, ConnectorShape>()
        {
            {typeof(bool), ConnectorShape.Triangle},
            {typeof(string), ConnectorShape.Circle},
            {typeof(double), ConnectorShape.Circle},
            {typeof(DateTime), ConnectorShape.Circle},
            {typeof(DataGrid), ConnectorShape.Square},
            {typeof(ServerConfig), ConnectorShape.RedSquare},
            {typeof(ControlFlow), ConnectorShape.RightTriangle},
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