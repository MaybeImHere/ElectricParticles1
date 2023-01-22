using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    internal class Vec2
    {
        public double x;
        public double y;

        public Vec2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Creates a zero vector.
        /// </summary>
        public Vec2() : this(0.0, 0.0) { }

        /// <summary>
        /// Returns the distance to another point.
        /// </summary>
        /// <param name="a">Point to measure distance to.</param>
        /// <returns>Distance.</returns>
        public double Dist(Vec2 a)
        {
            double a1 = a.x - x;
            double a2 = a.y - y;
            return Math.Sqrt(a1 * a1 + a2 * a2);
        }

        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            return new Vec2(a.x + b.x, a.y + b.y);
        }

        public static Vec2 operator +(Vec2 a, double b)
        {
            return new Vec2(a.x + b, a.y + b);
        }

        public static Vec2 operator -(Vec2 a, Vec2 b)
        {
            return new Vec2(a.x - b.x, a.y - b.y);
        }

        public static Vec2 operator -(Vec2 a)
        {
            return new Vec2(-a.x, -a.y);
        }

        public static Vec2 operator *(Vec2 a, double b)
        {
            return new Vec2(a.x * b, a.y * b);
        }

        /// <summary>
        /// Divides both x and y by a double.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>A Vec2 with the new value.</returns>
        /// <exception cref="DivideByZeroException">Thrown if the double is zero.</exception>
        public static Vec2 operator /(Vec2 a, double b)
        {
            if (b == 0.0) throw new DivideByZeroException();

            return new Vec2(a.x / b, a.y / b);
        }

        /// <summary>
        /// Gets the angle to `other` with the current Vec2 being the center point of rotation.
        /// </summary>
        /// <param name="other">Other point to compare relatively to.</param>
        /// <returns></returns>
        public double GetTheta(Vec2 other)
        {
            return Math.Atan2(other.y - y, other.x - x);
        }

        /// <summary>
        /// Returns a unit vector in the direction of angle.
        /// </summary>
        /// <param name="angle">Angle in radians. Right-horizontal is 0 rads.</param>
        /// <returns></returns>
        public static Vec2 GetUnitVectorAngle(double angle)
        {
            return new Vec2(Math.Cos(angle), Math.Sin(angle));
        }

        public override string ToString()
        {
            return "(" + x.ToString("N3") + ", " + y.ToString("N3") + ")";
        }

        public string ToDesmosString()
        {
            return x.ToString("F3") + "\t" + y.ToString("F3");
        }

        public Vector2f Vec2V2f()
        {
            return new Vector2f((float)x, (float)y);
        }

        public static readonly Vec2 Zero = new Vec2();
        public static readonly Vec2 One = new Vec2(1.0, 1.0);

    }
}
