using Parcel.Neo.Shared.Framework.ViewModels;
using Parcel.Neo.Shared.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Shared.Framework
{
    public interface IWebPreviewProcessorNode
    {
        #region Interface
        void OpenWebPreview(string target = "Preview")
        {
            if (WebHostRuntime.Singleton != null && this is ProcessorNode processorNode)
            {
                WebHostRuntime.Singleton.LastNode = processorNode;
                WebHostRuntime.Singleton.OpenTarget(target);
            }
        }
        #endregion
    }
}