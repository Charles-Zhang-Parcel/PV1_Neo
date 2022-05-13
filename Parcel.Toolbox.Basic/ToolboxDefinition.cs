﻿using System.Reflection;
using Parcel.Shared.Framework;
using Parcel.Shared.Framework.Advanced;
using Parcel.Shared.Framework.ViewModels.BaseNodes;
using Parcel.Shared.Framework.ViewModels.Primitives;
using Parcel.Toolbox.Basic.Nodes;

namespace Parcel.Toolbox.Basic
{
    public class ToolboxDefinition: IToolboxEntry
    {
        #region Interface
        public string ToolboxName => "Basic";
        public string ToolboxAssemblyFullName => Assembly.GetExecutingAssembly().FullName;
        public ToolboxNodeExport[] ExportNodes => new ToolboxNodeExport[]
        {
            new ToolboxNodeExport("Comment", typeof(CommentNode)),
            new ToolboxNodeExport("Preview", typeof(Preview)),
            null, // Divisor line // Primitive Nodes
            new ToolboxNodeExport("Number", typeof(NumberNode)),
            new ToolboxNodeExport("String", typeof(StringNode)),
            new ToolboxNodeExport("Boolean", typeof(BooleanNode)),
            new ToolboxNodeExport("DateTime", typeof(DateTimeNode)),
            new ToolboxNodeExport("File", typeof(OpenFileNode)),
            new ToolboxNodeExport("Array", typeof(object)), // Generic array representation of all above types, CANNOT have mixed types
            null, // Divisor line // Graph Modularization
            new ToolboxNodeExport("Graph Input", typeof(GraphInput)),
            new ToolboxNodeExport("Graph Output", typeof(GraphOutput)),
            new ToolboxNodeExport("Graph Reference", typeof(GraphReference)),
            new ToolboxNodeExport("Sub Graph", typeof(object)),
        };
        public AutomaticNodeDescriptor[] AutomaticNodes => new AutomaticNodeDescriptor[] { };
        #endregion
    }
}