using System;
using Parcel.Neo.Base.Framework.ViewModels.Primitives;

namespace Parcel.Neo.Base.DataTypes
{
    [Serializable]
    /// <summary>
    /// This will be a subset of <seealso cref="CacheDataType"/>
    /// </summary>
    public enum DictionaryEntryType
    {
        Number,
        String,
        Boolean
    }

    // Remark-cz: Do we have to have those? Can we just do raw types? Maybe it's provided just for the sake of front-end (that would make sense because Gospel has something similar?
    [Serializable]
    public enum CacheDataType
    {
        // Primitive
        Boolean,
        Number,
        String,
        DateTime,
        // Basic Numerical
        ParcelDataGrid, // Including arrays
        // Advanced
        Generic,
        BatchJob,
        ServerConfig
    }

    public static class CacheTypeHelper
    {
        public static CacheDataType ConvertToCacheDataType(Type type)
        {
            if (type == typeof(double))
                return CacheDataType.Number;
            else if (type == typeof(float))
                return CacheDataType.Number;
            else if (type == typeof(int))
                return CacheDataType.Number;
            else if (type == typeof(long))
                return CacheDataType.Number;
            else if (type == typeof(string))
                return CacheDataType.String;
            throw new ArgumentException($"Unrecognized type: {type.Name}");
        }
        public static Type ConvertToNodeType(CacheDataType type)
        {
            switch (type)
            {
                case CacheDataType.Boolean:
                    return typeof(BooleanNode);
                case CacheDataType.Number:
                    return typeof(NumberNode);
                case CacheDataType.String:
                    return typeof(StringNode);
                case CacheDataType.DateTime:
                    return typeof(DateTimeNode);
                case CacheDataType.ParcelDataGrid:
                    return typeof(DataGrid);
                case CacheDataType.Generic:
                    return typeof(object);
                case CacheDataType.BatchJob:
                case CacheDataType.ServerConfig:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public static Type ConvertToObjectType(CacheDataType type)
        {
            switch (type)
            {
                case CacheDataType.Boolean:
                    return typeof(bool);
                case CacheDataType.Number:
                    return typeof(double);
                case CacheDataType.String:
                    return typeof(string);
                case CacheDataType.DateTime:
                    return typeof(DateTime);
                case CacheDataType.ParcelDataGrid:
                    return typeof(DataGrid);
                case CacheDataType.Generic:
                    return typeof(object);
                case CacheDataType.BatchJob:
                case CacheDataType.ServerConfig:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static Type ConvertToNodeType(Type dataType)
        {
            if (dataType == typeof(double))
                return typeof(NumberNode);
            else if (dataType == typeof(string))
                return typeof(StringNode);
            else if (dataType == typeof(bool))
                return typeof(BooleanNode);
            else if (dataType == typeof(DateTime))
                return typeof(DateTimeNode);
            throw new ArgumentException("Advanced data type not supported.");
        }
    }
}