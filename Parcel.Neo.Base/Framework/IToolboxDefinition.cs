namespace Parcel.Neo.Base.Framework
{
    public interface IToolboxDefinition
    {
        public string? ToolboxName { get; }
        public string? ToolboxAssemblyFullName { get; }
        public ToolboxNodeExport?[]? ExportNodes { get; }
        public AutomaticNodeDescriptor[]? AutomaticNodes { get; }
    }

    public sealed class GenericToolbox: IToolboxDefinition
    {
        public string? ToolboxName { get; set;  }
        public string? ToolboxAssemblyFullName { get; set; }
        public ToolboxNodeExport?[]? ExportNodes { get; set; }
        public AutomaticNodeDescriptor[]? AutomaticNodes { get; set; }
    }
}