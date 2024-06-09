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
                return DataTypes.CacheDataType.Number;
            throw new ArgumentException();
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