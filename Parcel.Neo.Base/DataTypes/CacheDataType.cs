using System;
using Parcel.Neo.Base.Framework.ViewModels.Primitives;
using Parcel.Types;

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
    // TODO: Pending removing this or moving it entirely to the front-end aka. PreviewWindow. At the moment the GUI (XAML) depends on i because of naive graph input/output definition support - which is quite restricting because it cannot handle arbitrary graph input/output types.
    /// <remarks>
    /// This is front-end level contract and entirely for the purpose of front-ends - unless a particular type implements Parcel.CoreEngine's serialization interface, and the front-end has a way to view that serialized result, all other particular visualization types must be explicitly supported here. That's the only way front-ends can provide reasonable preview for any specific object type (e.g. data grid and images).
    /// </remarks>
    [Serializable]
    public enum CacheDataType
    {
        // Primitive
        Boolean,
        Number,
        String,
        DateTime,
        // Basic Types
        ParcelDataGrid, // Including arrays // TODO/REMARK-cz: in general, even though this is defined here, front-ends (depending on what type of front-end it is) do not necessarily need to depend on Parcel.DataGrid package
        ParcelDataGridDataColumn,
        // BitmapImage, // Not implemented
        // Advanced (Not implemented)
        Generic,
        BatchJob
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
            else if (type == typeof(DataGrid))
                return CacheDataType.ParcelDataGrid;
            else if (type == typeof(DataColumn) || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))) // The second part deals with nullable
                return CacheDataType.ParcelDataGridDataColumn;
            else if (type == typeof(bool))
                return CacheDataType.Boolean;
            else // Object
                return CacheDataType.Generic;
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