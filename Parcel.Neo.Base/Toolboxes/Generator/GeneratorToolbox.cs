using System;
using System.Linq;
using Parcel.Types;
using Parcel.Neo.Base.Framework;
using RandomNameGeneratorNG;

namespace Parcel.Neo.Base.Toolboxes.Generator
{
    public class GeneratorToolbox : IToolboxDefinition
    {
        #region Interface
        public ToolboxNodeExport?[]? ExportNodes => AutomaticNodes.Select(a => a == null ? null : new ToolboxNodeExport(a.NodeName, a)).ToArray();

        public AutomaticNodeDescriptor?[]? AutomaticNodes => [
            // Random Numbers
            new("Random Number", [], typeof(double),
                objects => new Random().NextDouble()),
            new("Random Integer in Range", [typeof(double), typeof(double)], typeof(double),
                objects => new Random().Next((int)(double)objects[0], (int)(double)objects[1])),
            new("Random Numbers", [typeof(double)], typeof(DataGrid),
                objects =>
                {
                    Random random = new();
                    return new DataGrid(Enumerable.Range(0, (int)(double)objects[0]).Select(_ => random.NextDouble()));
                }),
            new("Random Integers in Range", [typeof(double), typeof(double), typeof(double)], typeof(DataGrid),
                objects =>
                {
                    Random random = new();
                    return new DataGrid(Enumerable.Range(0, (int)(double)objects[0]).Select(_ => random.Next((int)(double)objects[1], (int)(double)objects[2])));
                })
            {
                InputNames = ["Count", "Start", "End"]
            },
            null, // Divisor line // Dates
            new("Today", [], typeof(DateTime),
                objects => DateTime.Today),
            null, // Divisor line // Strings
            new("Random Name", [], typeof(string),
                objects => new PersonNameGenerator().GenerateRandomFirstAndLastName()),
            new("Random Names", [typeof(double)], typeof(DataGrid),
                objects => new DataGrid(new PersonNameGenerator().GenerateMultipleFirstAndLastNames((int)(double)objects[0])))
        ];
        #endregion
    }
}