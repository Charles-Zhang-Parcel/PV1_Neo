using Parcel.Neo.Shared.Framework.ViewModels.BaseNodes;

namespace Parcel.Neo.Shared.Framework.ViewModels.Primitives
{
    public class StringNode: PrimitiveNode
    {
        public StringNode()
        {
            Title = NodeTypeName = "String";
        }

        public override OutputConnector MainOutput => ValueOutput as OutputConnector;
    }
}