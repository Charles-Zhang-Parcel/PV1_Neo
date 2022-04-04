﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Parcel.Shared.Framework;
using Parcel.Shared.Framework.ViewModels;
using Parcel.Shared.Framework.ViewModels.BaseNodes;
using DataGrid = Parcel.Shared.DataTypes.DataGrid;

namespace Parcel.FrontEnd.NodifyWPF
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

        #region Routines
        private void GeneratePreviewForOutput()
        {
            WindowGrid.Children.Clear();
            InfoGridVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Collapsed;
            
            OutputConnector output = Node.MainOutput;
            if (Node.ProcessorCache.ContainsKey(output))
            {
                ConnectorCacheDescriptor cache = Node.ProcessorCache[output];
                switch (cache.DataType)
                {
                    case CacheDataType.Boolean:
                        TestLabel = $"{cache.DataObject}";
                        InfoGridVisibility = Visibility.Visible;
                        break;
                    case CacheDataType.Number:
                        TestLabel = $"{cache.DataObject}";
                        InfoGridVisibility = Visibility.Visible;
                        break;
                    case CacheDataType.String:
                        TestLabel = $"{cache.DataObject}";
                        InfoGridVisibility = Visibility.Visible;
                        break;
                    case CacheDataType.ParcelDataGrid:
                        PopulateDataGrid((cache.DataObject as DataGrid).Rows);
                        DataGridVisibility = Visibility.Visible;
                        break;
                    default:
                        TestLabel = "No preview is available for this node.";
                        InfoGridVisibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void PopulateDataGrid(List<dynamic> objects)
        {
            dynamic test = new ExpandoObject();
            test.Name = "Name";
            test.Value = "Value";
            
            // Collect column names
            IEnumerable<IDictionary<string, object>> rows = objects.OfType<IDictionary<string, object>>();
            IEnumerable<string> columns = rows.SelectMany(d => d.Keys).Distinct(StringComparer.OrdinalIgnoreCase);
            // Generate columns
            WpfDataGrid.Columns.Clear();
            foreach (string text in columns)
            {
                // now set up a column and binding for each property
                var column = new DataGridTextColumn 
                {
                    Header = text.Trim().Trim('"'),
                    Binding = new Binding(text)
                };
                WpfDataGrid.Columns.Add(column);
            }

            // Bind object
            DataGridData = objects;
        }

        #endregion
    }
}