using System.Collections.Generic;
using System.Linq;
using Parcel.Neo.Base.DataTypes;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Toolbox.DataProcessing.Nodes
{
    /// <summary>
    /// Sum up all numerical columns and produce a new data grid
    /// </summary>
    public class Sum: ProcessorNode
    {
        #region Node Interface
        private readonly InputConnector _dataTableInput = new InputConnector(typeof(DataGrid))
        {
            Title = "Data Table",
        };
        private readonly OutputConnector _dataTableOutput = new OutputConnector(typeof(DataGrid))
        {
            Title = "Result",
        };
        public Sum()
        {
            Title = NodeTypeName = "Sum";
            Input.Add(_dataTableInput);
            Output.Add(_dataTableOutput);
        }
        #endregion
        
        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            DataGrid dataGrid = _dataTableInput.FetchInputValue<DataGrid>();
            DataGrid result = new DataGrid();
            foreach (DataColumn dataColumn in dataGrid.Columns.Where(c => c.Type == typeof(double)))
            {
                DataColumn newColumn = new DataColumn($"{dataColumn.Header} (Sum)");
                newColumn.Add(dataColumn.Sum());
                result.Columns.Add(newColumn);
            }

            return new NodeExecutionResult(new NodeMessage($"{result.Columns.Count} Numerical Columns"), new Dictionary<OutputConnector, object>()
            {
                {_dataTableOutput, result}
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