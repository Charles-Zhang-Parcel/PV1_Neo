using System;
using System.Collections.Generic;
using Parcel.Neo.Base.DataTypes;
using Parcel.Types;

namespace Parcel.Neo.Base.Framework
{
    public readonly struct ConnectorCache
    {
        // Remark-cz: Do we have to have those? Can we just do raw types?
        private static readonly Dictionary<Type, CacheDataType> DataTypeMapping = new Dictionary<Type, CacheDataType>()
        {
            {typeof(double), CacheDataType.Number},
            {typeof(int), CacheDataType.Number},
            {typeof(float), CacheDataType.Number},
            {typeof(bool), CacheDataType.Boolean},
            {typeof(string), CacheDataType.Boolean},
            {typeof(DateTime), CacheDataType.DateTime},
            {typeof(DataGrid), CacheDataType.ParcelDataGrid},
        };

        #region Main Data
        public object DataObject { get; }
        public CacheDataType DataType { get; }
        #endregion

        #region Construction
        public ConnectorCache(object dataObject)
        {
            DataObject = dataObject;

            if (dataObject != null)
            {
                Type type = dataObject.GetType();
                DataType = DataTypeMapping.TryGetValue(type, out CacheDataType value)
                    ? value
                    : CacheDataType.Generic; // TODO: or we should potentially throw an error    
            }
            else DataType = CacheDataType.Generic;
        }
        public ConnectorCache(object dataObject, CacheDataType dataType)
        {
            DataObject = dataObject;
            DataType = dataType;
        }
        #endregion

        #region Accessor
        public bool IsAvailable => DataObject != null;
        #endregion
    }
}