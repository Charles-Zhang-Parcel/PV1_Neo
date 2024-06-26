﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
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

        #region Events
        private void PreviewWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();    // Allow only LMB, since RMB can cause an exception
        }
        #endregion

        #region Routines
        private static readonly Type[] PrimitiveTypes = [
            typeof(string),
            typeof(bool),
            typeof(byte),
            typeof(int),
            typeof(float),
            typeof(double),
        ];
        private void GeneratePreviewForOutput()
        {
            WindowGrid.Children.Clear();
            StringDisplayVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Collapsed;
            
            OutputConnector output = Node.MainOutput;
            if (Node.HasCache(output))
            {
                ConnectorCache cache = Node[output];
                if (cache.DataType.IsArray && PrimitiveTypes.Any(t => t.IsAssignableFrom(cache.DataType.GetElementType()))) // This should not be necessary since the handling of IList should have already handled it
                    PreviewPrimitiveArray((Array)cache.DataObject);
                else if (cache.DataObject is System.Collections.IList list)
                    PreviewCollections(list);
                else if (PrimitiveTypes.Contains(cache.DataType))
                    PreviewPrimitives(cache.DataObject);
                else if (cache.DataType == typeof(Types.DataGrid))
                    PreviewDataGrid(cache.DataObject as Parcel.Types.DataGrid);
                else if (cache.DataType == typeof(Types.Image))
                {
                    Types.Image image = (cache.DataObject as Types.Image)!;
                    string? address = image.FileReference;
                    if (address != null && System.IO.File.Exists(address))
                        PreviewImage(new BitmapImage(new Uri(address)));
                    else
                        PreviewImage(ConvertToBitmapImage(image));
                }
                else if (cache.DataType == typeof(DataColumn))
                    PreviewColumnData(cache.DataObject as Parcel.Types.DataColumn);
                else
                {
                    TestLabel = $"No preview is available for this node's output ({cache.DataObject})";
                    StringDisplayVisibility = Visibility.Visible;
                }
            }
        }
        #endregion

        #region Preview Functions
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
        private void PreviewPrimitiveArray(Array array)
        {
            PopulateDataGrid(WpfDataGrid, new Types.DataGrid("Values", array), out string[] dataGridDataColumns, out List<dynamic> dataGridData);
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

        private void PreviewImage(ImageSource imageSource)
        {
            PreviewImageVisibility = Visibility.Visible;
            PreviewImageControl.Source = imageSource;

            // Automatically adjust preview window size
            Width = PreviewImageControl.Source.Width;
            Height = PreviewImageControl.Source.Height;
        }
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
                DataGridTextColumn column = new()
                {
                    Header = FormatHeader(columnName, columnInfo[columnName].TypeName),
                    Binding = new Binding(columnName)
                };
                wpfDataGrid.Columns.Add(column);
            }

            // Bind object
            dataGridData = objects;
        }
        private static ImageSource ConvertToBitmapImage(Types.Image image)
        {
            // Remark-cz: This is slightly hacky because at the moment we cannot find a reliable way to conver Bitmap directly into WPF recognizable ImageSource and honestly it's API is very sick and I don't want to bother.
            string tempPath = GetTempImagePath();
            image.ConvertParcelImageToBitmap().Save(tempPath);
            return new BitmapImage(new Uri(tempPath));
        }
        private static string GetTempImagePath()
            => Path.GetTempPath() + Guid.NewGuid().ToString() + ".png";
        #endregion
    }

    public static class ConvertBitmapToBitmapImageHelper
    {
        #region Bitmap to Bitmap Image
        /// <summary>
        /// Takes a bitmap and converts it to an image that can be handled by WPF ImageBrush
        /// DOES NOT WORK
        /// </summary>
        public static BitmapImage ConvertBitmapToBitmapImage(this Bitmap bitmap)
        {
            using MemoryStream memoryStream = new();
            bitmap.Save(memoryStream, ImageFormat.Bmp);

            BitmapImage bitmapImage = new();
            bitmapImage.BeginInit();
            memoryStream.Seek(0, SeekOrigin.Begin);
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
            return bitmapImage;
        }
        /// <summary>
        /// Not tested; Likely DOES NOT WORK
        /// </summary>
        public static BitmapImage ConvertBitmapToBitmapImage(this byte[] data)
        {
            using MemoryStream memoryStream = new(data);
            BitmapImage bitmapImage = new();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // CacheOption must be set after BeginInit()
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
            if (bitmapImage.CanFreeze)
                bitmapImage.Freeze();
            return bitmapImage;
        }
        #endregion

        #region Raw Bitmap Construction
        public static Bitmap CopyDataToBitmap(byte[] data, int width, int height, System.Drawing.Imaging.PixelFormat pixelFormat = System.Drawing.Imaging.PixelFormat.Format24bppRgb)
        {
            Bitmap bitmap = new(width, height, pixelFormat);

            // Copy the data from the byte array into BitmapData.Scan0 while lock all pixels to be written 
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
            Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);

            // Unlock the pixels and return the bitmap
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }
        #endregion

        #region Parcel Specific Conversion Helper
        public static Bitmap ConvertParcelImageToBitmap(this Types.Image image)
        {
            static byte[] RearrangeBytes(Pixel[][] pixels, int columns, int rows)
            {
                int stride = sizeof(byte) * 4;
                byte[] bytes = new byte[columns * rows * stride];

                for (int row = 0; row < rows; row++)
                {
                    Pixel[] rowPixels = pixels[row];
                    for (int col = 0; col < columns; col++)
                    {
                        Pixel pixel = rowPixels[col];

                        // Bitmap is row major but in RGRA (most likely because it's taking an integer internally (which is dumb, not documented and not specified) and because we are on intel little-endian architecture
                        // TODO: Below setting might not be platform independent and may have problem on big-endian systems
                        bytes[row * columns * stride + col * stride + 0] = pixel.Blue;
                        bytes[row * columns * stride + col * stride + 1] = pixel.Green;
                        bytes[row * columns * stride + col * stride + 2] = pixel.Red;
                        bytes[row * columns * stride + col * stride + 3] = pixel.Alpha; // Alpha
                    }
                }
                return bytes;
            }

            byte[] alignedBytes = RearrangeBytes(image.Pixels, image.Width, image.Height);
            Bitmap bitmap = new(image.Width, image.Height, image.Width * 4,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb,
                Marshal.UnsafeAddrOfPinnedArrayElement(alignedBytes, 0));
            return bitmap;
        }
        #endregion
    }
}