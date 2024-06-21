using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite; // TODO: Change to Microsoft SQLite
using System.Dynamic;
using System.IO;
using System.Linq;
using ExcelDataReader;
using Parcel.CoreEngine.Helpers;
using Parcel.Types;
using DataColumn = Parcel.Types.DataColumn;
using DataTable = System.Data.DataTable;

namespace Parcel.Neo.Base.Toolboxes.DataProcessing
{
    #region Parameters
    public class TabularFileBaseParameter
    {
        public string InputPath { get; set; }
        public bool InputContainsHeader { get; set; }
        public DataGrid OutputTable { get; set; }
    }
    public class CSVParameter : TabularFileBaseParameter
    {
    }
    public class ExcelParameter : TabularFileBaseParameter
    {
    }
    public class TakeParameter
    {
        public DataGrid InputTable { get; set; }
        public int InputRowCount { get; set; }
        public DataGrid OutputTable { get; set; }
    }
    public class ExtractParameter
    {
        public DataGrid InputTable { get; set; }
        public string[] InputColumnNames { get; set; }
        public DataGrid OutputTable { get; set; }
    }
    public class ExcludeParameter
    {
        public DataGrid InputTable { get; set; }
        public string[] InputColumnNames { get; set; }
        public DataGrid OutputTable { get; set; }
    }
    public class RenameParameter
    {
        public DataGrid InputTable { get; set; }
        public string[] InputColumns { get; set; }
        public string[] InputColumnNewNames { get; set; }
        public DataGrid OutputTable { get; set; }
    }
    public class SortParameter
    {
        public DataGrid InputTable { get; set; }
        public string InputColumnName { get; set; }
        public bool InputReverseOrder { get; set; }
        public DataGrid OutputTable { get; set; }
    }
    public class AppendParameter
    {
        public DataGrid[] InputTables { get; set; }
        public DataGrid OutputTable { get; set; }
    }
    public class TransposeParameter
    {
        public DataGrid InputTable { get; set; }
        public DataGrid OutputTable { get; set; }
    }
    public class MatrixMultiplyParameter
    {
        public DataGrid[] InputTables { get; set; }
        public bool[] InputTableShouldTransposes { get; set; }
        public DataGrid OutputTable { get; set; }
    }
    public class SQLParameter
    {
        public DataGrid[] InputTables { get; set; }
        public string[] InputTableNames { get; set; }
        public string InputCommand { get; set; }
        public DataGrid OutputTable { get; set; }
    }
    #endregion

    public static class DataProcessingHelper
    {
        public static void CSV(CSVParameter parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter.InputPath) || !File.Exists(parameter.InputPath))
                throw new ArgumentException("Invalid inputs");
            // TODO: Currently if the CSV File is opened by excel then it's not readable by us, and the File.ReadAllText will throw an exception

            bool hasHeader = parameter.InputContainsHeader ? true : false;
            parameter.OutputTable = new DataGrid(Path.GetFileNameWithoutExtension(parameter.InputPath), CSVHelper.ReadCSVFile(parameter.InputPath, out string[] headers, hasHeader), headers);
        }

        public static void Excel(ExcelParameter parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter.InputPath) || !File.Exists(parameter.InputPath))
                throw new ArgumentException("Invalid inputs");

            DataSet result = null;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (var stream = File.Open(parameter.InputPath, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                result = reader.AsDataSet();
            }

            parameter.OutputTable = new DataGrid(result, parameter.InputContainsHeader);
        }

        public static void Take(TakeParameter parameter)
        {
            if (parameter.InputTable == null)
                throw new ArgumentException("Missing Data Table input.");
            if (parameter.InputRowCount <= 0)
                throw new ArgumentException("Invalid take amount");
            if (parameter.InputRowCount > parameter.InputTable.RowCount)
                throw new ArgumentException("Take amount is more than amount of rows in table.");
            if (parameter.InputRowCount == parameter.InputTable.RowCount)
                throw new ArgumentException("Take amount is the same as the amount of rows in table.");

            DataGrid newDataGrid = new();
            foreach (DataColumn inputTableColumn in parameter.InputTable.Columns)
                newDataGrid.AddColumnFrom(inputTableColumn, parameter.InputRowCount);
            parameter.OutputTable = newDataGrid;
        }

        public static void Extract(ExtractParameter parameter)
        {
            if (parameter.InputTable == null)
                throw new ArgumentException("Missing Data Table input.");
            if (parameter.InputTable != null && parameter.InputColumnNames.Length == 0)
                throw new ArgumentException("No columns are given for the table.");

            Dictionary<string, DataGrid.ColumnInfo> columnInfos = parameter.InputTable.GetColumnInfoForDisplay();
            if (parameter.InputColumnNames.Any(cn => !columnInfos.ContainsKey(cn)))
                throw new ArgumentException("Cannot find column with specified name on data table.");

            parameter.OutputTable = parameter.InputTable.Extract(parameter.InputColumnNames.Select(cn => columnInfos[cn].ColumnIndex));
        }

        public static void Exclude(ExcludeParameter parameter)
        {
            if (parameter.InputTable == null)
                throw new ArgumentException("Missing Data Table input.");
            if (parameter.InputColumnNames.Length == 0)
                throw new ArgumentException("No columns are given for the table.");

            Dictionary<string, DataGrid.ColumnInfo> columnInfos = parameter.InputTable.GetColumnInfoForDisplay();
            if (parameter.InputColumnNames.Any(cn => !columnInfos.ContainsKey(cn)))
                throw new ArgumentException("Cannot find column with specified name on data table.");

            parameter.OutputTable = parameter.InputTable.Exclude(parameter.InputColumnNames.Select(cn => columnInfos[cn].ColumnIndex));
        }

        public static void Rename(RenameParameter parameter)
        {
            if (parameter.InputTable == null)
                throw new ArgumentException("Missing Data Table input.");
            if (parameter.InputColumns.Any(string.IsNullOrWhiteSpace)
                || parameter.InputColumnNewNames.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Empty inputs.");

            Dictionary<string, DataGrid.ColumnInfo> columnInfos = parameter.InputTable.GetColumnInfoForDisplay();
            if (parameter.InputColumns.Any(c => !columnInfos.ContainsKey(c)))
                throw new ArgumentException("Input column name doesn't exist on input table.");
            // TODO: At the moment we are allowing renamed column names to already exist on the table, i.e. columns with same headers

            DataGrid copy = parameter.InputTable.MakeCopy();
            for (int i = 0; i < parameter.InputColumns.Length; i++)
            {
                int oldColumnIndex = columnInfos[parameter.InputColumns[i]].ColumnIndex;
                string newName = parameter.InputColumnNewNames[i];
                copy.Columns[oldColumnIndex].RenameHeader(newName);
            }
            parameter.OutputTable = copy;
        }

        public static void Sort(SortParameter parameter)
        {
            if (parameter.InputTable == null)
                throw new ArgumentException("Missing Data Table input.");
            if (parameter.InputTable != null && string.IsNullOrWhiteSpace(parameter.InputColumnName))
                throw new ArgumentException("No column selection is given for the table");
            if (parameter.InputTable != null && !string.IsNullOrWhiteSpace(parameter.InputColumnName)
                                             && parameter.InputTable.Columns.All(c => c.Header != parameter.InputColumnName))
                throw new ArgumentException("Cannot find column with specified name on data table");

            var newTable = parameter.InputTable.MakeCopy();
            newTable.Sort(parameter.InputColumnName, parameter.InputReverseOrder);
            parameter.OutputTable = newTable;
        }

        public static void Append(AppendParameter parameter)
        {
            if (parameter.InputTables.Any(t => t == null))
                throw new ArgumentException("Missing Data Table input.");
            if (parameter.InputTables.Select(t => t.RowCount).Distinct().Count() != 1)
                throw new ArgumentException("Data Table size are not equal (there might be bad data).");

            DataGrid result = parameter.InputTables.First();
            for (int i = 1; i < parameter.InputTables.Length; i++)
                result = result.MakeCopy().Append(parameter.InputTables[i]);
            parameter.OutputTable = result;
        }

        public static void Transpose(TransposeParameter parameter)
        {
            if (parameter.InputTable == null)
                throw new ArgumentException("Missing Data Table input.");

            parameter.OutputTable = parameter.InputTable.Transpose();
        }

        public static void MatrixMultiply(MatrixMultiplyParameter parameter)
        {
            if (parameter.InputTables.Length != parameter.InputTableShouldTransposes.Length)
                throw new ArgumentException("Wrong number of inputs.");
            if (parameter.InputTables.Any(t => t == null))
                throw new ArgumentException("Invalid table inputs.");

            DataGrid result = parameter.InputTableShouldTransposes.First() ? parameter.InputTables.First().Transpose() : parameter.InputTables.First();
            for (int i = 1; i < parameter.InputTables.Length; i++)
            {
                bool shouldTranspose = parameter.InputTableShouldTransposes[i];
                result = result.MatrixMultiply(shouldTranspose ? parameter.InputTables[i].Transpose() : parameter.InputTables[i]);
            }
            parameter.OutputTable = result;
        }

        public static void SQL(SQLParameter parameter)
        {
            void PopulateTable(DataGrid table, string tableName, SQLiteConnection connection)
            {
                SQLiteCommand cmd = connection.CreateCommand();
                cmd.CommandText = $"CREATE TABLE '{tableName}'({string.Join(',', table.Columns.Select(c => $"'{c.Header}'"))})";
                cmd.ExecuteNonQuery();

                // Remark: The API is as shitty as it can get

                SQLiteTransaction transaction = connection.BeginTransaction();

                string sql = $"select * from '{tableName}' limit 1";
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, connection);
                adapter.InsertCommand = new SQLiteCommandBuilder(adapter).GetInsertCommand();

                DataSet dataSet = new DataSet();
                adapter.FillSchema(dataSet, SchemaType.Source, tableName);
                adapter.Fill(dataSet, tableName);     // Load exiting table data (will be empty) 

                // Insert data
                DataTable dataTable = dataSet.Tables[tableName];
                foreach (ExpandoObject row in table.Rows)
                {
                    DataRow dataTableRow = dataTable.NewRow();
                    foreach (KeyValuePair<string, dynamic> pair in (IDictionary<string, dynamic>)row)
                        dataTableRow[pair.Key] = pair.Value;
                    dataTable.Rows.Add(dataTableRow);
                }
                int result = adapter.Update(dataTable);

                transaction.Commit();
                dataSet.AcceptChanges();
                // Release resources 
                adapter.Dispose();
                dataSet.Clear();
            }

            if (parameter.InputTables.Length == 0 || parameter.InputTables == null)
                throw new ArgumentException("Missing Data Table input.");

            using (var connection = new SQLiteConnection("Data Source=:memory:"))
            {
                connection.Open();

                // Initialize
                for (int i = 0; i < parameter.InputTables.Length; i++)
                {
                    PopulateTable(parameter.InputTables[i], parameter.InputTableNames[i], connection);
                }

                // Execute
                string formattedText = parameter.InputCommand.EndsWith(';')
                    ? parameter.InputCommand
                    : parameter.InputCommand + ';';
                for (int i = 0; i < parameter.InputTables.Length; i++)
                    formattedText = formattedText.Replace($"@Table{i + 1}", $"'{parameter.InputTableNames[i]}'"); // Table names can't use parameters, so do it manually
                SQLiteDataAdapter adapter = new(formattedText, connection);
                DataSet result = new();
                adapter.Fill(result);
                // using (var reader = command.ExecuteReader())
                // {
                //     while (reader.Read())
                //     {
                //         var name = reader.GetString(0);
                //
                //         Console.WriteLine($"Hello, {name}!");
                //     }
                // }

                parameter.OutputTable = new DataGrid(result);
                connection.Close();
            }
        }
    }
}