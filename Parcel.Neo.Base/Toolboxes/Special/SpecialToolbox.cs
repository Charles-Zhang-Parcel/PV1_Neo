﻿using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Toolboxes.Special.Nodes;

namespace Parcel.Neo.Base.Toolboxes.Special
{
    public class SpecialToolbox : IToolboxDefinition
    {
        #region Interface
        public ToolboxNodeExport[] ExportNodes => [
            // Special - Specialized Graph Visualization
            new("Graph Stats", typeof(GraphStats)),
            //new("Console Output", typeof(object)), // With options to specify how many lines to show
            //new("Python Snippet", typeof(object)), // With auto binding inputs and outputs
            //null, // Divisor line // Utility
            //new("Graph Attributes", typeof(object)),
            //null, // Divisor line // Decoration
            //new("Header", typeof(object)),
            //new("Text", typeof(object)),
            //new("URL", typeof(object)),
            //new("Image", typeof(object)),
            //new("Markdown", typeof(object)),
            //new("Audio", typeof(object)),
            //new("Web Page", typeof(object)),
            //new("Help Page", typeof(object)),
            //null, // Divisor line // Others
            //new("Contact", typeof(object)),
            //new("About", typeof(object)),
        ];
        #endregion
    }
}