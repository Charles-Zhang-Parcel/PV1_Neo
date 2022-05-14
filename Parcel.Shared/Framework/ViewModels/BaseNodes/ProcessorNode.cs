﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Parcel.Shared.DataTypes;

namespace Parcel.Shared.Framework.ViewModels.BaseNodes
{
    public interface IMainOutputNode
    {
        public OutputConnector MainOutput { get; }
    }
    public abstract class ProcessorNode: BaseNode, IProcessor, IMainOutputNode, IAutoConnect
    {
        #region Public View Properties - State
        private string _title;
        public string Title
        {
            get => !string.IsNullOrWhiteSpace(_title) ? _title : _nodeTypeName;
            set => SetField(ref _title, value);
        }
        private bool _isPreview;
        public bool IsPreview
        {
            get => _isPreview;
            set => SetField(ref _isPreview, value);
        }
        #endregion

        #region Public View Properties - Node Type
        private string _nodeTypeName;
        public string NodeTypeName
        {
            get => _nodeTypeName;
            set => SetField(ref _nodeTypeName, value);
        }
        private string _tooltip;
        public string Tooltip
        {
            get => _tooltip;
            set => SetField(ref _tooltip, value);
        }
        #endregion

        #region Public View Properties - Transient State
        private NodeMessage _message = new NodeMessage();
        public NodeMessage Message
        {
            get => _message;
            set => SetField(ref _message, value);
        }
        #endregion

        #region Accesor
        public string FullName => (!string.IsNullOrWhiteSpace(_title) && _title != _nodeTypeName) ? $"({_nodeTypeName}) {_title}" : _nodeTypeName;
        #endregion

        #region Connectors
        public NotifyObservableCollection<InputConnector> Input { get; } = new NotifyObservableCollection<InputConnector>();
        public NotifyObservableCollection<OutputConnector> Output { get; } = new NotifyObservableCollection<OutputConnector>();
        #endregion

        #region Interface
        public ProcessorNode()
        {
            BaseProcessorMemberSerialization = new Dictionary<string, NodeSerializationRoutine>()
            {
                {nameof(Title), new NodeSerializationRoutine(() => _title, value => _title = value as string)},
                {nameof(IsPreview), new NodeSerializationRoutine(() => _isPreview, value => _isPreview = (bool)value)},
            };
            
            Input.WhenAdded(c => c.Node = this)
                .WhenRemoved(c => c.Disconnect());

            Output.WhenAdded(c => c.Node = this)
                .WhenRemoved(c => c.Disconnect());
        }
        public void Disconnect()
        {
            Input.Clear();
            Output.Clear();
        }
        public virtual OutputConnector MainOutput => Output.Count == 0 ? null : Output[0];
        public void Evaluate()
        {
            NodeExecutionResult result = this.Execute();

            if (result.Message != null)
            {
                Message.Content = result.Message.Content;
                Message.Type = result.Message.Type;
            }
            
            if (result.Caches == null) return;
            foreach ((OutputConnector outputConnector, object value) in result.Caches)
                ProcessorCache[outputConnector] = value is ConnectorCacheDescriptor descriptor ? descriptor : new ConnectorCacheDescriptor(value);
        }
        public ConnectorCacheDescriptor this[OutputConnector cacheConnector] => ProcessorCache[cacheConnector];
        public bool HasCache(OutputConnector cacheConnector) => ProcessorCache.ContainsKey(cacheConnector);
        #endregion

        #region Routines
        protected abstract NodeExecutionResult Execute();
        private Dictionary<OutputConnector, ConnectorCacheDescriptor> ProcessorCache { get; } =
            new Dictionary<OutputConnector, ConnectorCacheDescriptor>();
        #endregion

        #region Auto Connect Interface
        public virtual bool ShouldHaveConnection => Input.Count != 0 && Input.First().Connections.Count == 0;

        public virtual Tuple<ToolboxNodeExport, Vector, InputConnector>[] AutoGenerateNodes
        {
            get
            {
                List<Tuple<ToolboxNodeExport, Vector, InputConnector>> auto =
                    new List<Tuple<ToolboxNodeExport, Vector, InputConnector>>();
                for (int i = 0; i < Input.Count; i++)
                {
                    if(Input[i].Connections.Count != 0 || Input[i].DataType == typeof(DataGrid)) continue;

                    ToolboxNodeExport toolDef = new ToolboxNodeExport(Input[i].Title, CacheTypeHelper.ConvertToNodeType(Input[i].DataType));
                    auto.Add(new Tuple<ToolboxNodeExport, Vector, InputConnector>(toolDef, new Vector(-180, -20 + (i - 1) * 50), Input[i]));
                }
                return auto.ToArray();
            }
        }

        #endregion

        #region Serialization
        public sealed override Dictionary<string, NodeSerializationRoutine> MemberSerialization =>
            BaseProcessorMemberSerialization.Select(d => d)
                .Union(ProcessorNodeMemberSerialization?.Select(d => d) ?? new KeyValuePair<string, NodeSerializationRoutine>[]{})
                .Union( InputConnectorsSerialization != null 
                    ? new [] {new KeyValuePair<string, NodeSerializationRoutine>(nameof(InputConnectorsSerialization), InputConnectorsSerialization)}
                    : new KeyValuePair<string, NodeSerializationRoutine>[]{})
                .ToDictionary(d => d.Key, d => d.Value);
        private Dictionary<string, NodeSerializationRoutine> BaseProcessorMemberSerialization { get; }
        protected abstract Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; }
        protected abstract NodeSerializationRoutine InputConnectorsSerialization { get; }
        public override int GetOutputPinID(OutputConnector connector) => Output.IndexOf(connector);
        public override int GetInputPinID(InputConnector connector) => Input.IndexOf(connector);
        public override BaseConnector GetOutputPin(int id) => Output[id];
        public override BaseConnector GetInputPin(int id) => Input[id];
        #endregion
    }
}