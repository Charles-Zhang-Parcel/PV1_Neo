using System.Reflection;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Toolboxes.DataProcessing.Nodes;

namespace Parcel.Neo.Base.Toolboxes.DataProcessing
{
    public class DataProcessingToolbox : IToolboxDefinition
    {
        #region Interface
        public string ToolboxName => "Data Processing";
        public string ToolboxAssemblyFullName => Assembly.GetExecutingAssembly().FullName;
        public ToolboxNodeExport[] ExportNodes => new[]
        {
            // Data Types and IO
            new ToolboxNodeExport("CSV", typeof(CSV)),
            new ToolboxNodeExport("Data Table", typeof(DataTable)), // DataTable or matrix initializer
            new ToolboxNodeExport("Dictionary", typeof(Dictionary)),
            new ToolboxNodeExport("Excel", typeof(Excel)),
            null, // Divisor line // High Level Operations
            new ToolboxNodeExport("Append", typeof(Append)),
            new ToolboxNodeExport("Extract & Reorder", typeof(Extract)),  // Can be used to extract or reorder fields
            new ToolboxNodeExport("Exclude", typeof(Exclude)),   // Opposite of Extract
            new ToolboxNodeExport("Rename", typeof(Rename)),
            new ToolboxNodeExport("Validate", typeof(object)),  // Validate and reinterpret formats
            new ToolboxNodeExport("Reinterpret", typeof(object)),  // Validate and reinterpret formats
            new ToolboxNodeExport("Sort", typeof(Sort)),
            new ToolboxNodeExport("Take", typeof(Take)),    // Similar to "trim"
            null, // Divisor line // Excel-Like Common
            // new ToolboxNodeExport("Reorder", typeof(object)), // Swap Fields; Same functionality as "Extract" 
            new ToolboxNodeExport("Aggregate (Pivot Table)", typeof(object)), // Like Excel Pivot Table; Output string as report; Pivot should be fully automatic
            new ToolboxNodeExport("Filter", typeof(object)),    // Like LINQ Where; Constrain by columns, return rows; Inputs can have multiple rows and columns for multi-search
            new ToolboxNodeExport("Search", typeof(object)),    // Like JQuery DataTable Search - will query through all fields, not constrained by columns
            new ToolboxNodeExport("Find Distinct Names", typeof(object)), // Find distinct of all non-numerical columns, Outout string as report
            new ToolboxNodeExport("Join", typeof(object)), // Automatic join two tables
            null, // Divisor line // Low Level Operations
            new ToolboxNodeExport("Add", typeof(object)),   // Add cell, add row, add column
            new ToolboxNodeExport("Convert", typeof(object)), // Act on individual columns
            new ToolboxNodeExport("Column Add", typeof(object)),
            new ToolboxNodeExport("Column Subtract", typeof(object)),
            new ToolboxNodeExport("Column Multiply", typeof(object)),
            new ToolboxNodeExport("Column Divide", typeof(object)),
            null, // Divisor Line // Basic Operations
            new ToolboxNodeExport("Sum", typeof(Sum)),
            null, // Divisor line // Matrix Operations
            new ToolboxNodeExport("Matrix Multiply", typeof(MatrixMultiply)), // Dynamic connector sequence, With option to transpose
            new ToolboxNodeExport("Matrix Scaling", typeof(object)), // Multiplication by a constant
            new ToolboxNodeExport("Matrix Addition", typeof(object)), // Add or subtract by a constant
            null, // Divisor line // Queries
            new ToolboxNodeExport("Names", typeof(object)), // Return string array of headers
            new ToolboxNodeExport("Size", typeof(object)), // Return integer count of rows and columns
            new ToolboxNodeExport("Count", typeof(object)), // Return count of an array
            null, // Divisor line // Data Conversion
            // new ToolboxNodeExport("To Matrix", typeof(object)), // TODO: Build all operations directly inside DataGrid
            new ToolboxNodeExport("Transpose", typeof(Transpose)),
            new ToolboxNodeExport("SQL Query", typeof(SQL)),
        };
        public AutomaticNodeDescriptor[] AutomaticNodes => new AutomaticNodeDescriptor[] { };
        #endregion
    }
}