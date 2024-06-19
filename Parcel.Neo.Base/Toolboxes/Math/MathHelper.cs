using System;

namespace Parcel.Neo.Base.Toolboxes.Math
{
    public static class MathHelper
    {
        public static double Add(double summand1, double summand2)
            => summand1 + summand2;
        public static double Subtract(double minuend, double subtrahend)
            => minuend - subtrahend;
        public static double Multiply(double factor1, double factor2)
            => factor1 * factor2;
        public static double Divide(double dividend, double divisor)
        {
            if (divisor == 0)
                throw new ArgumentException("Second input cannot be zero.");

            return dividend / divisor;
        }
        public static double Modulus(double dividend, double divisor)
            => dividend % divisor;
        public static double Power(double @base, double exponent)
            => System.Math.Pow(@base, exponent);
        public static double Root(double radicand, double degree)
            => System.Math.Pow(radicand, 1.0 / degree); // Mathematically equivalent
        public static double Log(double antilogarithm, double @base)
            => System.Math.Log(antilogarithm, @base);
        public static double Sin(double angle)
            => System.Math.Sin(angle);
    }
}