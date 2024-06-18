using System.Collections.Generic;
using System.Linq;
using Parcel.Neo.Base.DataTypes;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Base.Serialization;

namespace Parcel.Toolbox.DataProcessing.Nodes
{
    public class DatabaseTableInputConnector: InputConnector
    {
        #region Properties
        private string _tableName = "New Key";
        public string TableName
        {
            get => _tableName;
            set => SetField(ref _tableName, value);
        }
        #endregion

        #region Constructor
        public DatabaseTableInputConnector(string tableName) : base(typeof(DataGrid))
        {
            TableName = tableName;
            Title = "Data Table";
        }
        #endregion
    }
    
    public class SQL: DynamicInputProcessorNode, INodeProperty
    {
        #region Node Interface
        private readonly OutputConnector _dataTableOutput = new OutputConnector(typeof(DataGrid))
        {
            Title = "Result"
        };
        private readonly OutputConnector _serverConfigOutput = new OutputConnector(typeof(ServerConfig))
        {
            Title = "Present"
        };
        public SQL()
        {
            // Serialization
            ProcessorNodeMemberSerialization = new Dictionary<string, NodeSerializationRoutine>()
            {
                {nameof(Code), new NodeSerializationRoutine(() => SerializationHelper.Serialize(Code), o => Code = SerializationHelper.GetString(o))}
            };
            VariantInputConnectorsSerialization = new NodeSerializationRoutine(() => SerializationHelper.Serialize(Input.Count), o =>
            {
                Input.Clear();
                int count = SerializationHelper.GetInt(o);
                for (int i = 0; i < count; i++)
                    AddInputs();
            });
            
            
            Editors = new List<PropertyEditor>()
            {
                new PropertyEditor("Code", PropertyEditorType.Code, () => _code, o => Code = (string)o)
            };
            
            Title = NodeTypeName = "SQL";
            Output.Add(_dataTableOutput);
            Output.Add(_serverConfigOutput);
            
            AddInputs();
            
            AddEntryCommand = new RequeryCommand(
                AddInputs,
                () => true);
            RemoveEntryCommand = new RequeryCommand(
                RemoveInputs,
                () => Input.Count > 1);
        }
        #endregion

        #region View Binding/Internal Node Properties
        private string _code = "select * from @Table1";
        public string Code
        {
            get => _code;
            set => SetField(ref _code, value);
        }
        #endregion

        #region Property Editor Interface
        public List<PropertyEditor> Editors { get; }
        #endregion

        #region Routines
        private void AddInputs()
        {
            Input.Add(new DatabaseTableInputConnector($"Table {Input.Count + 1}"));
        }
        private void RemoveInputs()
        {
            Input.RemoveAt(Input.Count - 1);
        }
        #endregion
        
        #region Processor Interface
        public override OutputConnector MainOutput => _dataTableOutput;
        protected override NodeExecutionResult Execute()
        {
            SQLParameter parameter = new SQLParameter()
            {
                InputTables = Input.Select(i =>
                {
                    DatabaseTableInputConnector connector = i as DatabaseTableInputConnector;
                    var table = connector!.FetchInputValue<DataGrid>();
                    // table.TableName = connector.TableName;
                    return table;
                }).ToArray(),
                InputTableNames = Input.Select(i => (i as DatabaseTableInputConnector)!.TableName).ToArray(),
                InputCommand = Code 
            };
            DataProcessingHelper.SQL(parameter);

            return new NodeExecutionResult(new NodeMessage($"{parameter.OutputTable.RowCount} Rows; {parameter.OutputTable.ColumnCount} Columns."), new Dictionary<OutputConnector, object>()
            {
                {_dataTableOutput, parameter.OutputTable}
            });
        }
        #endregion
        
        #region Serialization
        protected override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; }
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; }
        #endregion
    }
}