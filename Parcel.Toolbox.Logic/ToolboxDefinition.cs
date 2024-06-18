using System.Reflection;
using Parcel.Neo.Base.DataTypes;
using Parcel.Neo.Base.Framework;
using Parcel.Toolbox.Logic.Nodes;

namespace Parcel.Toolbox.Logic
{
    public class ToolboxDefinition: IToolboxDefinition
    {
        #region Interface
        public string ToolboxName => "Logic";
        public string ToolboxAssemblyFullName => Assembly.GetExecutingAssembly().FullName;
        public ToolboxNodeExport[] ExportNodes => new ToolboxNodeExport[]
        {
            // Functional
            new ToolboxNodeExport("Choose", typeof(Choose)),
        };
        public AutomaticNodeDescriptor[] AutomaticNodes => new AutomaticNodeDescriptor[]
        {
            // Numerical
            new AutomaticNodeDescriptor("> (Bigger Than)", new []{CacheDataType.Number, CacheDataType.Number}, CacheDataType.Number, 
                objects => (double)objects[0] > (double)objects[1]),
            new AutomaticNodeDescriptor("< (Smaller Than)", new []{CacheDataType.Number, CacheDataType.Number}, CacheDataType.Number, 
                objects => (double)objects[0] < (double)objects[1]),
            null, // Divisor line // Boolean
            new AutomaticNodeDescriptor("AND", new []{CacheDataType.Boolean, CacheDataType.Boolean}, CacheDataType.Boolean, 
                objects => (bool)objects[0] && (bool)objects[1]),
            new AutomaticNodeDescriptor("OR", new []{CacheDataType.Boolean, CacheDataType.Boolean}, CacheDataType.Boolean, 
                objects => (bool)objects[0] || (bool)objects[1]),
        };
        #endregion
    }
}