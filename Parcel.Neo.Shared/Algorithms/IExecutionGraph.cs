using System.Collections.Generic;
using Parcel.Neo.Shared.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Shared.Algorithms
{
    public interface IExecutionGraph
    {
        public void InitializeGraph(IEnumerable<ProcessorNode> targetNodes);
        public void ExecuteGraph();
    }
}