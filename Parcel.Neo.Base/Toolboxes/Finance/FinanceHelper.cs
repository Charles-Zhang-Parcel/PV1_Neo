using System;
using System.Linq;
using Parcel.Types;

namespace Parcel.Neo.Base.Toolboxes.Finance
{
    #region Parameters
    // Remark-cz: The use of such "Parameters" looks redundant - handlign routines can just take whatever arguments they need directly
    public class RobustMeanParameter
    {
        public double[] InputList { get; set; }
        public DataGrid InputTable { get; set; }
        public string InputColumnName { get; set; }

        /// <remarks>GUI can provide more specific dynamic behavior for this parameter based on Table Input,
        /// e.g. hide it when InputTable is not hooked, and provide enumeration if input table contains headers</remarks>
        public int InputColumnSelection { get; set; } = -1;

        public double[] OutputList { get; set; }
        public DataGrid OutputTable { get; set; }
        public double OutputValue { get; set; }
    }

    public class BaseSingleColumnOperationParameter
    {
        public DataGrid InputTable { get; set; }
        public string InputColumnName { get; set; }
        public double OutputValue { get; set; }
    }
    public class BaseTwoTableOperationParameter
    {
        public DataGrid InputTable1 { get; set; }
        public string InputColumnName1 { get; set; }
        public DataGrid InputTable2 { get; set; }
        public string InputColumnName2 { get; set; }
        public double OutputValue { get; set; }
    }
    public class MeanParameter : BaseSingleColumnOperationParameter { }
    public class VarianceParameter : BaseSingleColumnOperationParameter { }
    public class StandardDeviationParameter : BaseSingleColumnOperationParameter { }
    public class MinParameter : BaseSingleColumnOperationParameter { }
    public class MaxParameter : BaseSingleColumnOperationParameter { }
    public class SumParameter : BaseSingleColumnOperationParameter { }
    public class CovarianceParameter : BaseTwoTableOperationParameter { }
    public class CorrelationParameter : BaseTwoTableOperationParameter { }
    public class CovarianceMatrixParameter
    {
        public DataGrid InputTable { get; set; }
        public DataGrid OutputTable { get; set; }
    }
    public class PercentReturnParameter
    {
        public DataGrid InputTable { get; set; }
        public bool LatestAtTop { get; set; }
        public DataGrid OutputTable { get; set; }
    }
    #endregion

    public static class FinanceHelper
    {
        public static void RobustMean(RobustMeanParameter parameter)
        {
            if (parameter.InputList == null && parameter.InputTable == null)
                throw new ArgumentException("Invalid inputs");
            if (parameter.InputList != null && parameter.InputTable != null)
                throw new ArgumentException("Invalid inputs");
            if (parameter.InputTable != null && parameter.InputColumnSelection == -1 && string.IsNullOrWhiteSpace(parameter.InputColumnName))
                throw new ArgumentException("No column selection is given for the table");
            if (parameter.InputTable != null && parameter.InputColumnSelection != -1
                                             && !string.IsNullOrWhiteSpace(parameter.InputColumnName)
                                             && parameter.InputTable.Columns.All(c => c.Header != parameter.InputColumnName))
                throw new ArgumentException("Cannot find column with specified name on data table");

            if (parameter.InputList != null)
                parameter.OutputValue = parameter.InputList.Sum();
            if (parameter.InputTable != null)
            {
                if (parameter.InputColumnSelection != -1)
                    parameter.OutputValue = parameter.InputTable.Columns[parameter.InputColumnSelection].Mean();
                else if (!string.IsNullOrWhiteSpace(parameter.InputColumnName))
                    parameter.OutputValue = parameter.InputTable.Columns.Single(c => c.Header == parameter.InputColumnName).Mean();
            }

            parameter.OutputList = parameter.InputList;
            parameter.OutputTable = parameter.InputTable;
        }

        public static void Mean(MeanParameter parameter)
        {
            if (parameter.InputTable == null)
                throw new ArgumentException("Missing Data Table input.");
            if (parameter.InputTable != null && string.IsNullOrWhiteSpace(parameter.InputColumnName))
                throw new ArgumentException("No column selection is given for the table");
            if (parameter.InputTable != null && !string.IsNullOrWhiteSpace(parameter.InputColumnName)
                                             && parameter.InputTable.Columns.All(c => c.Header != parameter.InputColumnName))
                throw new ArgumentException("Cannot find column with specified name on data table");

            parameter.OutputValue = parameter.InputTable.Columns.Single(c => c.Header == parameter.InputColumnName).Mean();
        }
        public static void Variance(VarianceParameter parameter)
        {
            if (parameter.InputTable == null)
                throw new ArgumentException("Missing Data Table input.");
            if (parameter.InputTable != null && string.IsNullOrWhiteSpace(parameter.InputColumnName))
                throw new ArgumentException("No column selection is given for the table");
            if (parameter.InputTable != null && !string.IsNullOrWhiteSpace(parameter.InputColumnName)
                                             && parameter.InputTable.Columns.All(c => c.Header != parameter.InputColumnName))
                throw new ArgumentException("Cannot find column with specified name on data table");

            var column = parameter.InputTable.Columns.Single(c => c.Header == parameter.InputColumnName);
            bool usePopulation = column.Length > 50;
            parameter.OutputValue = column.Variance(usePopulation);
        }

        public static void StandardDeviation(StandardDeviationParameter parameter)
        {
            if (parameter.InputTable == null)
                throw new ArgumentException("Missing Data Table input.");
            if (parameter.InputTable != null && string.IsNullOrWhiteSpace(parameter.InputColumnName))
                throw new ArgumentException("No column selection is given for the table");
            if (parameter.InputTable != null && !string.IsNullOrWhiteSpace(parameter.InputColumnName)
                                             && parameter.InputTable.Columns.All(c => c.Header != parameter.InputColumnName))
                throw new ArgumentException("Cannot find column with specified name on data table");

            var column = parameter.InputTable.Columns.Single(c => c.Header == parameter.InputColumnName);
            bool usePopulation = column.Length > 50;
            parameter.OutputValue = column.StandardDeviation(usePopulation);
        }

        public static void Min(MinParameter parameter)
        {
            if (parameter.InputTable == null)
                throw new ArgumentException("Missing Data Table input.");
            if (parameter.InputTable != null && string.IsNullOrWhiteSpace(parameter.InputColumnName))
                throw new ArgumentException("No column selection is given for the table");
            if (parameter.InputTable != null && !string.IsNullOrWhiteSpace(parameter.InputColumnName)
                                             && parameter.InputTable.Columns.All(c => c.Header != parameter.InputColumnName))
                throw new ArgumentException("Cannot find column with specified name on data table");

            parameter.OutputValue = parameter.InputTable.Columns.Single(c => c.Header == parameter.InputColumnName).Min();
        }

        public static void Max(MaxParameter parameter)
        {
            if (parameter.InputTable == null)
                throw new ArgumentException("Missing Data Table input.");
            if (parameter.InputTable != null && string.IsNullOrWhiteSpace(parameter.InputColumnName))
                throw new ArgumentException("No column selection is given for the table");
            if (parameter.InputTable != null && !string.IsNullOrWhiteSpace(parameter.InputColumnName)
                                             && parameter.InputTable.Columns.All(c => c.Header != parameter.InputColumnName))
                throw new ArgumentException("Cannot find column with specified name on data table");

            parameter.OutputValue = parameter.InputTable.Columns.Single(c => c.Header == parameter.InputColumnName).Max();
        }

        public static void Sum(SumParameter parameter)
        {
            if (parameter.InputTable == null)
                throw new ArgumentException("Missing Data Table input.");
            if (parameter.InputTable != null && string.IsNullOrWhiteSpace(parameter.InputColumnName))
                throw new ArgumentException("No column selection is given for the table");
            if (parameter.InputTable != null && !string.IsNullOrWhiteSpace(parameter.InputColumnName)
                                             && parameter.InputTable.Columns.All(c => c.Header != parameter.InputColumnName))
                throw new ArgumentException("Cannot find column with specified name on data table");

            parameter.OutputValue = parameter.InputTable.Columns.Single(c => c.Header == parameter.InputColumnName).Sum();
        }

        public static void Correlation(CorrelationParameter parameter)
        {
            if (parameter.InputTable1 == null || parameter.InputTable2 == null)
                throw new ArgumentException("Missing Data Table input.");
            if (parameter.InputTable1 != null && string.IsNullOrWhiteSpace(parameter.InputColumnName1) && parameter.InputTable1.Columns.Count != 1
                || parameter.InputTable2 != null && string.IsNullOrWhiteSpace(parameter.InputColumnName2) && parameter.InputTable2.Columns.Count != 1)
                throw new ArgumentException("No column selection is given for the table");
            if (parameter.InputTable1 != null && !string.IsNullOrWhiteSpace(parameter.InputColumnName1)
                                               && parameter.InputTable1.Columns.All(c => c.Header != parameter.InputColumnName1)
                || parameter.InputTable2 != null && !string.IsNullOrWhiteSpace(parameter.InputColumnName2)
                                                  && parameter.InputTable2.Columns.All(c => c.Header != parameter.InputColumnName2))
                throw new ArgumentException("Cannot find column with specified name on data table");

            var column1 = parameter.InputTable1.Columns.Single(c => string.IsNullOrWhiteSpace(parameter.InputColumnName1) || c.Header == parameter.InputColumnName1);
            var column2 = parameter.InputTable1.Columns.Single(c => string.IsNullOrWhiteSpace(parameter.InputColumnName2) || c.Header == parameter.InputColumnName2);
            parameter.OutputValue = column1.Correlation(column2);
        }

        public static void Covariance(CovarianceParameter parameter)
        {
            if (parameter.InputTable1 == null || parameter.InputTable2 == null)
                throw new ArgumentException("Missing Data Table input.");
            if (parameter.InputTable1 != null && string.IsNullOrWhiteSpace(parameter.InputColumnName1) && parameter.InputTable1.Columns.Count != 1
                 || parameter.InputTable2 != null && string.IsNullOrWhiteSpace(parameter.InputColumnName2) && parameter.InputTable2.Columns.Count != 1)
                throw new ArgumentException("No column selection is given for the table");
            if (parameter.InputTable1 != null && !string.IsNullOrWhiteSpace(parameter.InputColumnName1)
                                              && parameter.InputTable1.Columns.All(c => c.Header != parameter.InputColumnName1)
                || parameter.InputTable2 != null && !string.IsNullOrWhiteSpace(parameter.InputColumnName2)
                                                 && parameter.InputTable2.Columns.All(c => c.Header != parameter.InputColumnName2))
                throw new ArgumentException("Cannot find column with specified name on data table");

            var column1 = parameter.InputTable1.Columns.Single(c => string.IsNullOrWhiteSpace(parameter.InputColumnName1) || c.Header == parameter.InputColumnName1);
            var column2 = parameter.InputTable1.Columns.Single(c => string.IsNullOrWhiteSpace(parameter.InputColumnName2) || c.Header == parameter.InputColumnName2);
            parameter.OutputValue = column1.Covariance(column2);
        }

        public static void CovarianceMatrix(CovarianceMatrixParameter parameter)
        {
            if (parameter.InputTable == null)
                throw new ArgumentException("Missing Data Table input.");

            parameter.OutputTable = parameter.InputTable.CovarianceMatrix();
        }

        public static void PercentReturn(PercentReturnParameter parameter)
        {
            if (parameter.InputTable == null)
                throw new ArgumentException("Missing Data Table input.");
            if (parameter.InputTable.Columns.Any(c => c.Type != typeof(DateTime)
                                                      && c.TypeName != "Number"
                                                      && c.Type != typeof(string)))
                throw new ArgumentException("Data Table contains invalid rows.");

            DataGrid result = new DataGrid();
            foreach (DataColumn sourceColumn in parameter.InputTable.Columns)
            {
                var resultColumn = result.AddColumn(sourceColumn.Header);
                if (parameter.LatestAtTop)
                {
                    for (int i = 0; i < sourceColumn.Length - 1; i++)
                    {
                        if (sourceColumn.Type == typeof(double))
                            resultColumn.Add((sourceColumn[i] - sourceColumn[i + 1]) / sourceColumn[i + 1]);
                        else
                            resultColumn.Add(sourceColumn[i]); // Ignore non-numerical column
                    }
                }
                else
                {
                    for (int i = sourceColumn.Length - 1; i > 0; i--)
                    {
                        if (sourceColumn.Type == typeof(double))
                            resultColumn.Add((sourceColumn[i] - sourceColumn[i - 1]) / sourceColumn[i - 1]);
                        else
                            resultColumn.Add(sourceColumn[i]); // Ignore non-numerical column
                    }
                }
            }

            parameter.OutputTable = result;
        }
    }
}