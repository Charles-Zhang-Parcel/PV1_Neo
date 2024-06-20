﻿using System;
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
        public ToolboxNodeExport?[]? ExportNodes => Array.Empty<ToolboxNodeExport>();
        public AutomaticNodeDescriptor?[]? AutomaticNodes => [
            // Random Numbers
            new("Random Number", [], CacheDataType.Number,
                objects => new Random().NextDouble()),
            new("Random Integer in Range", [CacheDataType.Number, CacheDataType.Number], CacheDataType.Number,
                objects => new Random().Next((int)(double)objects[0], (int)(double)objects[1])),
            new("Random Numbers", [CacheDataType.Number], CacheDataType.ParcelDataGrid,
                objects =>
                {
                    Random random = new();
                    return new DataGrid(Enumerable.Range(0, (int)(double)objects[0]).Select(_ => random.NextDouble()));
                }),
            new("Random Integers in Range", [CacheDataType.Number, CacheDataType.Number, CacheDataType.Number], CacheDataType.ParcelDataGrid,
                objects =>
                {
                    Random random = new();
                    return new DataGrid(Enumerable.Range(0, (int)(double)objects[0]).Select(_ => random.Next((int)(double)objects[1], (int)(double)objects[2])));
                })
            {
                InputNames = ["Count", "Start", "End"]
            },
            null, // Divisor line // Dates
            new("Today", [], CacheDataType.DateTime,
                objects => DateTime.Today),
            null, // Divisor line // Strings
            new("Random Name", [], CacheDataType.String,
                objects => new PersonNameGenerator().GenerateRandomFirstAndLastName()),
            new("Random Names", [CacheDataType.Number], CacheDataType.ParcelDataGrid,
                objects => new DataGrid(new PersonNameGenerator().GenerateMultipleFirstAndLastNames((int)(double)objects[0])))
        ];
        #endregion
    }
}