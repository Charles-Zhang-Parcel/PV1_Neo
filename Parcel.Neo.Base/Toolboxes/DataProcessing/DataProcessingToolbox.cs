using Parcel.CoreEngine.Runtime;
using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Toolboxes.DataProcessing.Nodes;

namespace Parcel.Neo.Base.Toolboxes.DataProcessing
{
    public class DataProcessingToolbox : IToolboxDefinition
    {
        #region Interface
        public ToolboxNodeExport?[]? ExportNodes => [
            // Data Types and IO
            new("CSV", RuntimeNodeType.Method, typeof(CSV)),
            new("Data Table", RuntimeNodeType.Method, typeof(DataTable)), // DataTable or matrix initializer
            new("Dictionary", RuntimeNodeType.Method, typeof(Dictionary)),
            new("Excel", RuntimeNodeType.Method, typeof(Excel)),
            null, // Divisor line // High Level Operations
            new("Append", RuntimeNodeType.Method, typeof(Append)),
            new("Extract & Reorder", RuntimeNodeType.Method, typeof(Extract)),  // Can be used to extract or reorder fields
            new("Exclude", RuntimeNodeType.Method, typeof(Exclude)),   // Opposite of Extract
            new("Rename", RuntimeNodeType.Method, typeof(Rename)),
            new("Validate", RuntimeNodeType.Method, typeof(object)),  // Validate and reinterpret formats
            new("Reinterpret", RuntimeNodeType.Method, typeof(object)),  // Validate and reinterpret formats
            new("Sort", RuntimeNodeType.Method, typeof(Sort)),
            // Take/Get/Extract Column
            // Take/Get/Extract Column by Name
            new("Take Rows", RuntimeNodeType.Method, typeof(TakeRows)),    // Similar to "trim"
            null, // Divisor line // Excel-Like Common
            // new("Reorder", typeof(object)), // Swap Fields; Same functionality as "Extract" 
            new("Aggregate (Pivot Table)", RuntimeNodeType.Method, typeof(object)), // Like Excel Pivot Table; Output string as report; Pivot should be fully automatic
            new("Filter", RuntimeNodeType.Method, typeof(object)),    // Like LINQ Where; Constrain by columns, return rows; Inputs can have multiple rows and columns for multi-search
            new("Search", RuntimeNodeType.Method, typeof(object)),    // Like JQuery DataTable Search - will query through all fields, not constrained by columns
            new("Find Distinct Names", RuntimeNodeType.Method, typeof(object)), // Find distinct of all non-numerical columns, Outout string as report
            new("Join", RuntimeNodeType.Method, typeof(object)), // Automatic join two tables
            null, // Divisor line // Low Level Operations
            new("Add", RuntimeNodeType.Method, typeof(object)),   // Add cell, add row, add column
            new("Convert", RuntimeNodeType.Method, typeof(object)), // Act on individual columns
            new("Column Add", RuntimeNodeType.Method, typeof(object)),
            new("Column Subtract", RuntimeNodeType.Method, typeof(object)),
            new("Column Multiply", RuntimeNodeType.Method, typeof(object)),
            new("Column Divide", RuntimeNodeType.Method, typeof(object)),
            null, // Divisor Line // Basic Operations
            new("Sum", RuntimeNodeType.Method, typeof(Sum)),
            null, // Divisor line // Matrix Operations
            new("Matrix Multiply", RuntimeNodeType.Method, typeof(MatrixMultiply)), // Dynamic connector sequence, With option to transpose
            new("Matrix Scaling", RuntimeNodeType.Method, typeof(object)), // Multiplication by a constant
            new("Matrix Addition", RuntimeNodeType.Method, typeof(object)), // Add or subtract by a constant
            null, // Divisor line // Queries
            new("Names", RuntimeNodeType.Method, typeof(object)), // Return string array of headers
            new("Size", RuntimeNodeType.Method, typeof(object)), // Return integer count of rows and columns
            new("Count", RuntimeNodeType.Method, typeof(object)), // Return count of an array
            null, // Divisor line // Data Conversion
            // new("To Matrix", RuntimeNodeType.Method, typeof(object)), // TODO: Build all operations directly inside DataGrid
            new("Transpose", RuntimeNodeType.Method, typeof(Transpose)),
            new("SQL Query", RuntimeNodeType.Method, typeof(SQL))
        ];
        #endregion
    }
}