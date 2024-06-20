using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Parcel.Types;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Base.Toolboxes.FileSystem.Nodes
{
    public class WriteCSV : ProcessorNode
    {
        #region Node Interface
        private readonly InputConnector _dataTableInput = new InputConnector(typeof(DataGrid))
        {
            Title = "Data Table",
        };
        private readonly InputConnector _pathInput = new PrimitiveStringInputConnector()
        {
            Title = "Path",
        };
        public WriteCSV()
        {
            Title = NodeTypeName = "Write CSV";
            Input.Add(_pathInput);  // This must be put before data table input because the front-end expects this order
            Input.Add(_dataTableInput);
        }
        #endregion

        #region Processor Interface
        protected override NodeExecutionResult Execute()
        {
            DataGrid dataGrid = _dataTableInput.FetchInputValue<DataGrid>();
            string path = _pathInput.FetchInputValue<string>();

            if (dataGrid == null)
                throw new ArgumentException("Missing data table input.");

            File.WriteAllText(path, dataGrid.ToCSV());
            if (File.Exists(path))
                Process.Start("explorer.exe", "/select, " + path);

            return new NodeExecutionResult(new NodeMessage($"Written to: {path}."), null);
        }
        #endregion

        #region Serialization
        protected override Dictionary<string, NodeSerializationRoutine> ProcessorNodeMemberSerialization { get; } =
            null;
        protected override NodeSerializationRoutine VariantInputConnectorsSerialization { get; } = null;
        #endregion

        #region Auto-Connect
        public override bool ShouldHaveAutoConnection => _pathInput.IsConnected == false && string.IsNullOrWhiteSpace(_pathInput.DefaultDataStorage as string);
        #endregion
    }
}