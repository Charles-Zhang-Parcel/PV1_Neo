﻿using Parcel.Neo.Base.Framework;

namespace Parcel.Toolbox.ControlFlow
{
    public class ControlFlowToolbox: IToolboxDefinition
    {
        #region Interface
        public ToolboxNodeExport?[]? ExportNodes => [
            //new ToolboxNodeExport("State Graph", typeof(object)),    // UE4 Blueprint like execution graph with explicit execution order and support for loops and states
            //null, // Divisor line
            //new ToolboxNodeExport("Action", typeof(object)),    // Like lambda, applied on each element in an array
            //new ToolboxNodeExport("Function", typeof(object)),  // A special node that actually refers to a graph with input and output markers; This graph MUST be stored directly within current graph to avoid unnecessary modularization (aka. we must implement supporting GUI)
            //null, // Divisor line
            //new ToolboxNodeExport("Apply", typeof(object)) // Like Map in JS or Select in C#, takes an Array and a Function node; equivalent as ForEach loop
            // Functional Logic
            // new ("Choose") // Choose based on input, like how it works in Houdini
        ];
        #endregion
    }
}