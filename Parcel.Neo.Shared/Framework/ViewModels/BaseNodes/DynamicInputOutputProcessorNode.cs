namespace Parcel.Neo.Shared.Framework.ViewModels.BaseNodes
{
    public abstract class DynamicInputProcessorNode : ProcessorNode
    {
        #region Input Entries
        public IProcessorNodeCommand AddEntryCommand { get; protected set; }
        public IProcessorNodeCommand RemoveEntryCommand { get; protected set; }
        #endregion
    }
}