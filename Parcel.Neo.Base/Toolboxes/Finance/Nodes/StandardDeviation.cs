﻿using System.Collections.Generic;
using Parcel.Types;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Toolboxes.Finance.Nodes
{
    public class StandardDeviation: ProcessorNode
    {
        #region Node Interface
        private readonly InputConnector _dataTableInput = new InputConnector(typeof(DataGrid))
        {
            Title = "Data Table",
        };
        private readonly InputConnector _columnNameInput = new InputConnector(typeof(string))
        {
            Title = "Column Name",
        };
        private readonly OutputConnector _valueOutput = new OutputConnector(typeof(double))
        {
            Title = "Value",
        };
        public StandardDeviation()
        {
            Title = NodeTypeName = "StandardDeviation";
            Input.Add(_dataTableInput);
            Input.Add(_columnNameInput);
            Output.Add(_valueOutput);
        }
        #endregion
        
        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            DataGrid dataGrid = _dataTableInput.FetchInputValue<DataGrid>();
            string columnName = _columnNameInput.FetchInputValue<string>();
            StandardDeviationParameter parameter = new StandardDeviationParameter()
            {
                InputTable = dataGrid,
                InputColumnName = columnName
            };
            FinanceHelper.StandardDeviation(parameter);

            return new NodeExecutionResult(new NodeMessage($"Standard Deviation={parameter.OutputValue}"), new Dictionary<OutputConnector, object>()
            {
                {_valueOutput, parameter.OutputValue}
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