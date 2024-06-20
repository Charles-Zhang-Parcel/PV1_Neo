using System;
using System.Collections.Generic;
using Parcel.Types;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Base.Framework.ViewModels.Primitives;
using Parcel.Neo.Base.DataTypes;

namespace Parcel.Neo.Base.Toolboxes.DataSource.Nodes
{
    public class YahooFinance : ProcessorNode
    {
        #region Node Interface
        private readonly InputConnector _symbolInput = new PrimitiveStringInputConnector()
        {
            Title = "Symbol",
        };
        private readonly InputConnector _startDateInput = new PrimitiveDateTimeInputConnector()
        {
            Title = "Start Date"
        };
        private readonly InputConnector _endDateInput = new PrimitiveDateTimeInputConnector()
        {
            Title = "End Date"
        };
        private readonly InputConnector _intervalInput = new PrimitiveStringInputConnector()
        {
            Title = "Interval",
        };
        private readonly OutputConnector _dataTableOutput = new OutputConnector(typeof(DataGrid))
        {
            Title = "Data Table"
        };
        public YahooFinance()
        {
            Title = NodeTypeName = "YahooFinance";
            Input.Add(_symbolInput);
            Input.Add(_startDateInput);
            Input.Add(_endDateInput);
            Input.Add(_intervalInput);
            Output.Add(_dataTableOutput);
        }
        #endregion

        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            YahooFinanceParameter parameter = new YahooFinanceParameter()
            {
                InputSymbol = _symbolInput.FetchInputValue<string>(),
                InputInterval = _intervalInput.FetchInputValue<string>(),
                InputStartDate = _startDateInput.FetchInputValue<DateTime>(),
                InputEndDate = _endDateInput.FetchInputValue<DateTime>()
            };
            DataSourceHelper.YahooFinance(parameter);

            return new NodeExecutionResult(new NodeMessage($"{parameter.OutputTable.RowCount} Rows; {parameter.OutputTable.ColumnCount} Columns."), new Dictionary<OutputConnector, object>()
            {
                {_dataTableOutput, parameter.OutputTable}
            });
        }
        #endregion

        #region Serialization
        protected override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; } =
            null;
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; } = null;
        #endregion

        #region Auto Connect Interface
        public override bool ShouldHaveAutoConnection => _symbolInput.Connections.Count == 0;
        public override Tuple<ToolboxNodeExport, Vector2D, InputConnector>[] AutoPopulatedConnectionNodes =>
            new Tuple<ToolboxNodeExport, Vector2D, InputConnector>[]
            {
                new Tuple<ToolboxNodeExport, Vector2D, InputConnector>(new ToolboxNodeExport("String", CoreEngine.Runtime.RuntimeNodeType.Method, typeof(StringNode)), new Vector2D(-250, -100), _symbolInput),
                new Tuple<ToolboxNodeExport, Vector2D, InputConnector>(new ToolboxNodeExport("Start Date", CoreEngine.Runtime.RuntimeNodeType.Method, typeof(DateTimeNode)), new Vector2D(-250, -50), _startDateInput),
                new Tuple<ToolboxNodeExport, Vector2D, InputConnector>(new ToolboxNodeExport("End Date", CoreEngine.Runtime.RuntimeNodeType.Method, typeof(DateTimeNode)), new Vector2D(-250, 0), _endDateInput),
                new Tuple<ToolboxNodeExport, Vector2D, InputConnector>(new ToolboxNodeExport("Interval", CoreEngine.Runtime.RuntimeNodeType.Method, typeof(StringNode)), new Vector2D(-250, 50), _intervalInput)
            };
        #endregion
    }
}