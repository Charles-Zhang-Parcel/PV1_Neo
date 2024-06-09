using System;
using NUnit.Framework;
using Parcel.Neo.Base.DataTypes;

namespace Parcel.Test.CoreTests.SharedFramework
{
    public class Tests
    {
        DataGrid TestDataGrid = new DataGrid();
        
        [SetUp]
        public void Setup()
        {
            TestDataGrid.AddColumn("Col1");
            TestDataGrid.AddColumn("Col2");
            TestDataGrid.AddColumn("Col3");
            TestDataGrid.AddColumn("Col4");
            TestDataGrid.AddRow(1, 2.0, "String", true);
            TestDataGrid.AddRow(1, 2.0, "String", false);
            TestDataGrid.AddRow(1, 2.0, "String", true);
            TestDataGrid.AddRow(1, 2.0, "String", false);
            TestDataGrid.AddRow(1, 2.0, "String", true);
        }

        #region Data Grid Storage
        /// <summary>
        /// DataGrid types should be "hidden" as programming model goes, but its internal types should be consistent.
        /// </summary>
        [Test]
        public void DataGridConsistentTypesRequirement()
        {
            Assert.Catch<ArgumentException>(() => TestDataGrid.AddRow("String Value", 0.2));
        }
        
        [Test]
        public void DataGridCorrectColumnTypes()
        {
            Assert.AreEqual(typeof(int), TestDataGrid.Columns[0].TypeName);
            Assert.AreEqual(typeof(double), TestDataGrid.Columns[1].TypeName);
            Assert.AreEqual(typeof(string), TestDataGrid.Columns[2].TypeName);
            Assert.AreEqual(typeof(bool), TestDataGrid.Columns[3].TypeName);
        }
        #endregion

        #region Data Grid Operations
        [Test]
        public void DataGridMeanOperation()
        {
            Assert.AreEqual(2.0, TestDataGrid.Columns[1].Mean());
        }
        #endregion
    }
}