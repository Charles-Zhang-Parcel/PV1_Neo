using System;
using System.Collections.Generic;
using Parcel.Neo.Shared.DataTypes;

namespace Parcel.Neo.Shared.Framework
{
    public readonly struct ConnectorCacheDescriptor
    {
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
        
        public object DataObject { get; }
        public CacheDataType DataType { get; }

        public ConnectorCacheDescriptor(object dataObject)
        {
            DataObject = dataObject;

            if (dataObject != null)
            {
                Type type = dataObject.GetType();
                DataType = DataTypeMapping.ContainsKey(type)
                    ? DataTypeMapping[type]
                    : CacheDataType.Generic; // TODO: or we should potentially throw an error    
            }
            else DataType = CacheDataType.Generic;
        }
        public ConnectorCacheDescriptor(object dataObject, CacheDataType dataType)
        {
            DataObject = dataObject;
            DataType = dataType;
        }

        public bool IsAvailable => DataObject != null;
    }
}