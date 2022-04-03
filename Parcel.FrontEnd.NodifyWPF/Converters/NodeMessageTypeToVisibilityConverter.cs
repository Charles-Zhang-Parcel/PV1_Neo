﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Nodify;
using Parcel.Shared.Framework;
using Parcel.Shared.Framework.ViewModels;

namespace Parcel.FrontEnd.NodifyWPF.Converters
{
    public class NodeMessageTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NodeMessageType messageType)
            {
                return messageType != NodeMessageType.Empty ? Visibility.Visible : Visibility.Collapsed;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Collapsed ? NodeMessageType.Empty : NodeMessageType.Normal;
            }

            return value;
        }
    }
}
