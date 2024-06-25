using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Framework.ViewModels;
using Parcel.Neo.Base.Framework.ViewModels.BaseNodes;
using Parcel.Types;

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

        private Visibility _previewImageVisibility = Visibility.Collapsed;
        public Visibility PreviewImageVisibility
        {
            get => _previewImageVisibility;
            set => SetField(ref _previewImageVisibility, value);
        }
        private Visibility _stringDisplayVisibility = Visibility.Visible;
        public Visibility StringDisplayVisibility
        {
            get => _stringDisplayVisibility;
            set => SetField(ref _stringDisplayVisibility, value);
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

        #region Configurations
        private const string ImageProtocolIdentifier = "Image://";
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
            StringDisplayVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Collapsed;
            
            OutputConnector output = Node.MainOutput;
            if (Node.HasCache(output))
            {
                ConnectorCache cache = Node[output];
                // Deal with special resource protocols
                if (cache.DataType == typeof(string) && (cache.DataObject as string).StartsWith(ImageProtocolIdentifier))
                {
                    string address = (cache.DataObject as string)[ImageProtocolIdentifier.Length..];
                    PreviewImage(address);
                }
                else if (cache.DataObject is System.Collections.IList list)
                    PreviewCollections(list);
                else if (cache.DataType == typeof(bool) || cache.DataType == typeof(string) || cache.DataType == typeof(double))
                    PreviewPrimitives(cache.DataObject);
                else if (cache.DataType == typeof(Types.DataGrid))
                    PreviewDataGrid(cache.DataObject as Parcel.Types.DataGrid);
                else if (cache.DataType == typeof(DataColumn))
                    PreviewColumnData(cache.DataObject as Parcel.Types.DataColumn);
                else
                {
                    TestLabel = $"No preview is available for this node's output ({cache.DataObject})";
                    StringDisplayVisibility = Visibility.Visible;
                }
            }
        }

        private void PreviewCollections(System.Collections.IList list)
        {
            PopulateDataGrid(WpfDataGrid, new Types.DataGrid("Values", list), out string[] dataGridDataColumns, out List<dynamic> dataGridData);
            DataGridDataColumns = dataGridDataColumns;
            DataGridData = dataGridData;
            DataGridVisibility = Visibility.Visible;
        }
        private void PreviewColumnData(DataColumn data)
        {
            PopulateDataGrid(WpfDataGrid, new Types.DataGrid("Preview", data), out string[] dataGridDataColumns, out List<dynamic> dataGridData);
            DataGridDataColumns = dataGridDataColumns;
            DataGridData = dataGridData;
            DataGridVisibility = Visibility.Visible;
        }

        private void PreviewDataGrid(Types.DataGrid data)
        {
            PopulateDataGrid(WpfDataGrid, data, out string[] dataGridDataColumns, out List<dynamic> dataGridData);
            DataGridDataColumns = dataGridDataColumns;
            DataGridData = dataGridData;
            DataGridVisibility = Visibility.Visible;
        }

        private void PreviewPrimitives(object data)
        {
            TestLabel = $"{data}";
            StringDisplayVisibility = Visibility.Visible;
        }

        private void PreviewImage(string address)
        {
            PreviewImageVisibility = Visibility.Visible;
            PreviewImageControl.Source = new BitmapImage(new Uri(address));

            // Automatically adjust preview window size
            Width = PreviewImageControl.Source.Width;
            Height = PreviewImageControl.Source.Height;
        }


        #endregion

        #region Preview Functions

        #endregion

        #region Helpers
        public static void PopulateDataGrid(System.Windows.Controls.DataGrid wpfDataGrid, Types.DataGrid dataGrid,
            out string[] dataGridDataColumns, out List<dynamic> dataGridData)
        {
            static string FormatHeader(string header, string typeName) 
                => $"{header} ({typeName})";

            List<dynamic> objects = dataGrid.Rows;
            Dictionary<string, Types.DataGrid.ColumnInfo> columnInfo = dataGrid.GetColumnInfoForDisplay();

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