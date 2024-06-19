using System;
using System.Linq;
using System.Reflection;
using Parcel.Neo.Base.DataTypes;
using Parcel.Neo.Base.Framework;
using RandomNameGeneratorNG;

namespace Parcel.Neo.Base.Toolboxes.Generator
{
    public class GeneratorToolbox : IToolboxDefinition
    {
        #region Interface
        public string ToolboxName => "Generator";
        public string ToolboxAssemblyFullName => Assembly.GetExecutingAssembly().FullName;
        public ToolboxNodeExport[] ExportNodes => new ToolboxNodeExport[] { };
        public AutomaticNodeDescriptor[] AutomaticNodes => new AutomaticNodeDescriptor[]
        {
            // Random Numbers
            new AutomaticNodeDescriptor("Random Number", new CacheDataType[]{}, CacheDataType.Number,
                objects => new Random().NextDouble()),
            new AutomaticNodeDescriptor("Random Integer in Range", new [] {CacheDataType.Number, CacheDataType.Number}, CacheDataType.Number,
                objects => new Random().Next((int)(double)objects[0], (int)(double)objects[1])),
            new AutomaticNodeDescriptor("Random Numbers", new []{CacheDataType.Number}, CacheDataType.ParcelDataGrid,
                objects =>
                {
                    Random random = new Random();
                    return new DataGrid(Enumerable.Range(0, (int)(double)objects[0]).Select(_ => random.NextDouble()));
                }),
            new AutomaticNodeDescriptor("Random Integers in Range", new [] {CacheDataType.Number, CacheDataType.Number, CacheDataType.Number}, CacheDataType.ParcelDataGrid,
                objects =>
                {
                    Random random = new Random();
                    return new DataGrid(Enumerable.Range(0, (int)(double)objects[0]).Select(_ => random.Next((int)(double)objects[1], (int)(double)objects[2])));
                })
            {
                InputNames = new []{"Count", "Start", "End"}
            },
            null, // Divisor line // Dates
            new AutomaticNodeDescriptor("Today", new CacheDataType[]{}, CacheDataType.DateTime,
                objects => DateTime.Today),
            null, // Divisor line // Strings
            new AutomaticNodeDescriptor("Random Name", new CacheDataType[]{}, CacheDataType.String,
                objects => new PersonNameGenerator().GenerateRandomFirstAndLastName()),
            new AutomaticNodeDescriptor("Random Names", new []{CacheDataType.Number}, CacheDataType.ParcelDataGrid,
                objects => new DataGrid(new PersonNameGenerator().GenerateMultipleFirstAndLastNames((int)(double)objects[0]))),
        };
        #endregion
    }
}