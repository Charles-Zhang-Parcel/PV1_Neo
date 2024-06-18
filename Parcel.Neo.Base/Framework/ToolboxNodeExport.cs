using System;

namespace Parcel.Neo.Base.Framework
{
    public class ToolboxNodeExport
    {
        #region Attributes
        public string Name { get; }
        public Type Type { get; }
        #endregion

        #region Additional Payloads
        public AutomaticNodeDescriptor Descriptor { get; set; }
        public IToolboxDefinition Toolbox { get; set; }
        #endregion

        public ToolboxNodeExport(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}