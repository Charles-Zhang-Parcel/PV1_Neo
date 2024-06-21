using Parcel.Neo.Base.Framework;
using Parcel.Neo.Base.Toolboxes.DataSource.Nodes;

namespace Parcel.Neo.Base.Toolboxes.DataSource
{
    public class DataSourceToolbox : IToolboxDefinition
    {
        #region Interface
        public ToolboxNodeExport?[]? ExportNodes => [
            // Data Base System
            //new("MS MDL", typeof(object)),
            //new("PL SQL", typeof(object)),
            //null, // Divisor line // Web Services
            new("Yahoo Finance", typeof(YahooFinance)),
        ];
        #endregion
    }
}