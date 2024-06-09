using System;
using Parcel.Neo.Shared.DataTypes;
using Parcel.Neo.Shared.Framework.ViewModels;

namespace Parcel.Neo.Shared.Framework
{
    public interface IAutoConnect
    {
        public bool ShouldHaveAutoConnection { get; }
        public Tuple<ToolboxNodeExport, Vector2D, InputConnector>[] AutoGenerateNodes { get; }
    }
}