using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using Parcel.Neo.Base.DataTypes;

namespace Parcel.Neo
{
    public partial class PreviewWindow : BaseWindow
    {
        #region Construction
        public PreviewWindow(Window owner, ProcessorNode processorNode)
        {
            Owner = owner;
            Node = processorNode;

            InitializeComponent();

            GeneratePreviewForOutput();
        }
        public ProcessorNode Node { get; }
        #endregion

        #region View Properties
        private string _testLabel;
        public string TestLabel
        {
            get => _testLabel;
            set => SetField(ref _testLabel, value);
        }

        private Visibility _infoGridVisibility = Visibility.Visible;
        public Visibility InfoGridVisibility
        {
            get => _infoGridVisibility;
            set => SetField(ref _infoGridVisibility, value);
        }
        private Visibility _dataGridVisibility = Visibility.Visible;
        public Visibility DataGridVisibility
        {
            get => _dataGridVisibility;
            set => SetField(ref _dataGridVisibility, value);
        }

        private string[] _dataGridDataColumns;
        public string[] DataGridDataColumns
        {
            get => _dataGridDataColumns;
            set => SetField(ref _dataGridDataColumns, value);
        }
        private List<dynamic> _dataGridData;
        public List<dynamic> DataGridData
        {
            get => _dataGridData;
            set => SetField(ref _dataGridData, value);
        }
        #endregion

        #region Interface
        public void Update()
        {
            GeneratePreviewForOutput();
            UpdateLayout();
        }
        #endregion

        #region Events
        private void PreviewWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();    // Allow only LMB, since RMB can cause an exception
        }
        #endregion

        #region Routines
        private void GeneratePreviewForOutput()
        {
            WindowGrid.Children.Clear();
            InfoGridVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Collapsed;
            
            OutputConnector output = Node.MainOutput;
            if (Node.HasCache(output))
            {
                ConnectorCache cache = Node[output];
                switch (cache.DataType)
                {
                    case CacheDataType.Boolean:
                    case CacheDataType.Number:
                    case CacheDataType.String:
                        TestLabel = $"{cache.DataObject}";
                        InfoGridVisibility = Visibility.Visible;
                        break;
                    case CacheDataType.ParcelDataGrid:
                        {
                            PopulateDataGrid(WpfDataGrid, cache.DataObject as Parcel.Types.DataGrid, out string[] dataGridDataColumns, out List<dynamic> dataGridData);
                            DataGridDataColumns = dataGridDataColumns;
                            DataGridData = dataGridData;
                            DataGridVisibility = Visibility.Visible;
                        }
                        break;
                    case CacheDataType.ParcelDataGridDataColumn:
                        {
                            PopulateDataGrid(WpfDataGrid, new Types.DataGrid("Preview", cache.DataObject as Parcel.Types.DataColumn), out string[] dataGridDataColumns, out List<dynamic> dataGridData);
                            DataGridDataColumns = dataGridDataColumns;
                            DataGridData = dataGridData;
                            DataGridVisibility = Visibility.Visible;
                        }
                        break;
                    case CacheDataType.Generic:
                    default:
                        TestLabel = $"No preview is available for this node's output ({cache.DataObject})";
                        InfoGridVisibility = Visibility.Visible;
                        break;
                }
            }
        }

        public static void PopulateDataGrid(DataGrid wpfDataGrid, Parcel.Types.DataGrid dataGrid,
            out string[] dataGridDataColumns, out List<dynamic> dataGridData)
        {
            string FormatHeader(string header, string typeName)
            {
                return $"{header} ({typeName})";
            }

            List<dynamic> objects = dataGrid.Rows;
            Dictionary<string, Parcel.Types.DataGrid.ColumnInfo> columnInfo = dataGrid.GetColumnInfoForDisplay();

            // Collect column names
            IEnumerable<IDictionary<string, object>> rows = objects.OfType<IDictionary<string, object>>();
            dataGridDataColumns = rows.SelectMany(d => d.Keys).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            // Generate columns
            wpfDataGrid.Columns.Clear();
            foreach (string columnName in dataGridDataColumns)
            {
                // now set up a column and binding for each property
                DataGridTextColumn column = new DataGridTextColumn 
                {
                    Header = FormatHeader(columnName, columnInfo[columnName].TypeName),
                    Binding = new Binding(columnName)
                };
                wpfDataGrid.Columns.Add(column);
            }

            // Bind object
            dataGridData = objects;
        }
        #endregion
    }
}