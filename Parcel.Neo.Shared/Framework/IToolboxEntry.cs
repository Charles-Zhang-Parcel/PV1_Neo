namespace Parcel.Neo.Shared.Framework
{
    public interface IToolboxEntry
    {
        public string ToolboxName { get; }
        public string ToolboxAssemblyFullName { get; }
        public ToolboxNodeExport[] ExportNodes { get; }
        public AutomaticNodeDescriptor[] AutomaticNodes { get; }
    }
}