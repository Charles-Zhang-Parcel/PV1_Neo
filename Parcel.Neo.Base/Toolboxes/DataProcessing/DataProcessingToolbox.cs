using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Toolboxes.DataProcessing.Nodes;

namespace Parcel.Neo.Base.Toolboxes.DataProcessing
{
    public class DataProcessingToolbox : IToolboxDefinition
    {
        #region Interface
        public ToolboxNodeExport?[]? ExportNodes => [
            // Data Types and IO
            new("Data Table", typeof(DataTable)), // DataTable or matrix initializer
            new("Dictionary", typeof(Dictionary)),
            null, // Divisor line // High Level Operations
            new("Validate", typeof(object)),  // Validate and reinterpret formats
            new("Reinterpret", typeof(object)),  // Validate and reinterpret formats
            // Take/Get/Extract Column
            // Take/Get/Extract Column by Name
            null, // Divisor line // Excel-Like Common
            // new("Reorder", typeof(object)), // Swap Fields; Same functionality as "Extract" 
            new("Aggregate (Pivot Table)", typeof(object)), // Like Excel Pivot Table; Output string as report; Pivot should be fully automatic
            new("Filter", typeof(object)),    // Like LINQ Where; Constrain by columns, return rows; Inputs can have multiple rows and columns for multi-search
            new("Search", typeof(object)),    // Like JQuery DataTable Search - will query through all fields, not constrained by columns
            new("Find Distinct Names", typeof(object)), // Find distinct of all non-numerical columns, Outout string as report
            new("Join", typeof(object)), // Automatic join two tables
            null, // Divisor line // Low Level Operations
            new("Add", typeof(object)),   // Add cell, add row, add column
            new("Convert", typeof(object)), // Act on individual columns
            new("Column Add", typeof(object)),
            new("Column Subtract", typeof(object)),
            new("Column Multiply", typeof(object)),
            new("Column Divide", typeof(object)),
            null, // Divisor line // Matrix Operations
            new("Matrix Scaling", typeof(object)), // Multiplication by a constant
            new("Matrix Addition", typeof(object)), // Add or subtract by a constant
            null, // Divisor line // Queries
            new("Names", typeof(object)), // Return string array of headers
            new("Size", typeof(object)), // Return integer count of rows and columns
            new("Count", typeof(object)), // Return count of an array
            null, // Divisor line // Data Conversion
            // new("To Matrix", typeof(object)), // TODO: Build all operations directly inside DataGrid
            new("SQL Query", typeof(SQL))
        ];
        #endregion
    }
}