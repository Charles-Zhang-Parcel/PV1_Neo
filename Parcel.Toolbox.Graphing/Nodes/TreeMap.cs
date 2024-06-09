using System.Collections.Generic;
using System.Linq;
using Parcel.Neo.Shared;
using Parcel.Neo.Shared.DataTypes;
using Parcel.Neo.Shared.Framework;
using Parcel.Neo.Shared.Framework.ViewModels;
using Parcel.Neo.Shared.Framework.ViewModels.BaseNodes;

namespace Parcel.Toolbox.Graphing.Nodes
{
    public class TreeMap: ProcessorNode, IWebPreviewProcessorNode, INodeProperty
    {
        #region Node Interface
        private readonly InputConnector _dataTableInput = new InputConnector(typeof(DataGrid))
        {
            Title = "Data Table"
        };
        private readonly InputConnector _tableNameInput = new PrimitiveStringInputConnector()
        {
            Title = "Display Name"
        };
        private readonly OutputConnector _serverConfigOutput = new OutputConnector(typeof(ServerConfig))
        {
            Title = "Present"
        };
        public TreeMap()
        {
            Editors = new List<PropertyEditor>()
            {
                new PropertyEditor("Parameters", PropertyEditorType.TextBox, () => _chartTitle, v => _chartTitle = (string)v)
            };
            
            Title = NodeTypeName = ChartTitle = "Tree Map";
            Input.Add(_dataTableInput);
            Input.Add(_tableNameInput);
            Output.Add(_serverConfigOutput);
        }
        #endregion

        #region View Binding/Internal Node Properties
        private string _chartTitle;
        public string ChartTitle
        {
            get => _chartTitle;
            set => SetField(ref _chartTitle, value);
        }
        #endregion
        
        #region Serialization
        protected override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; } =
            null;
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; } = null;
        #endregion

        #region Property Editor Interface
        public List<PropertyEditor> Editors { get; }
        #endregion
        
        #region Processor Interface
        public override OutputConnector MainOutput => _serverConfigOutput;

        protected override NodeExecutionResult Execute()
        {
            var dataGrid = _dataTableInput.FetchInputValue<DataGrid>();
            ServerConfig config = new ServerConfig()
            {
                ChartType = ChartType.TreeMap,
                ContentType = CacheDataType.ParcelDataGrid,
                LayoutSpec = LayoutElementType.GridGraph,
                DataGridContent = dataGrid,
                ObjectContent = _tableNameInput.FetchInputValue<string>()
            };
            WebHostRuntime.Singleton.CurrentLayout = config;
            WebHostRuntime.Singleton.DataTableEndPoints.Add("TreeMap", dataGrid);
            
            ((IWebPreviewProcessorNode)this).OpenWebPreview("Present");
            return new NodeExecutionResult(new NodeMessage($"Ready."), new Dictionary<OutputConnector, object>()
            {
                {_serverConfigOutput, config}
            });
        }
        #endregion
    }
}