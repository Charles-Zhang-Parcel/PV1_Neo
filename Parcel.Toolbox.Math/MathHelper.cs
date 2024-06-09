using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using Csv;
using Parcel.Neo.Shared.DataTypes;
using DataColumn = Parcel.Neo.Shared.DataTypes.DataColumn;
using DataTable = System.Data.DataTable;

namespace Parcel.Toolbox.Math
{
    #region Parameters
    public class BaseNumericalParameter
    {
        public double InputNumber1 { get; set; }
        public double InputNumber2 { get; set; }
        public double OutputNumber { get; set; }
    }
    public class AddParameter: BaseNumericalParameter
    {
    }
    public class SubtractParameter: BaseNumericalParameter
    {
    }
    public class MultiplyParameter: BaseNumericalParameter
    {
    }
    public class DivideParameter: BaseNumericalParameter
    {
    }
    public class ModulusParameter: BaseNumericalParameter
    {
    }
    public class PowerParameter: BaseNumericalParameter
    {
    }

    public class SinParameter
    {
        public double InputAngle { get; set; }
        public double OutputNumber { get; set; }
    }
    #endregion

    public static class MathHelper
    {
        public static void Add(AddParameter parameter)
        {
            parameter.OutputNumber = parameter.InputNumber1 + parameter.InputNumber2;
        }
        
        public static void Subtract(SubtractParameter parameter)
        {
            parameter.OutputNumber = parameter.InputNumber1 - parameter.InputNumber2;
        }
        
        public static void Multiply(MultiplyParameter parameter)
        {
            parameter.OutputNumber = parameter.InputNumber1 * parameter.InputNumber2;
        }
        
        public static void Divide(DivideParameter parameter)
        {
            if (parameter.InputNumber2 == 0)
                throw new ArgumentException("Second input cannot be zero.");

            parameter.OutputNumber = parameter.InputNumber1 / parameter.InputNumber2;
        }
        
        public static void Modulus(ModulusParameter parameter)
        {
            parameter.OutputNumber = parameter.InputNumber1 % parameter.InputNumber2;
        }
        
        public static void Power(PowerParameter parameter)
        {
            parameter.OutputNumber = System.Math.Pow(parameter.InputNumber1, parameter.InputNumber2);
        }
        
        public static void Sin(SinParameter parameter)
        {
            parameter.OutputNumber = System.Math.Sin(parameter.InputAngle);
        }
    }
}