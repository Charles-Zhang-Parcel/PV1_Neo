namespace Parcel.Neo.Shared.DataTypes
{
    public struct Vector2D
    {
        public double X;
        public double Y;

        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Vector2D operator +(Vector2D a, Vector2D b) => new Vector2D(a.X + b.X, a.Y + b.Y);
    }
}
