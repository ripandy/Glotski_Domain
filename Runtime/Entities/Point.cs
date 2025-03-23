using System;

namespace Glotski
{
    [Serializable]
    public struct Point : IEquatable<Point>
    {
        public int x;
        public int y;
    
        public static Point zero = new(0, 0);
        
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    
        public bool Equals(Point other)
        {
            return x == other.x && y == other.y;
        }
    
        public override bool Equals(object obj)
        {
            return obj is Point other && Equals(other);
        }
    
        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }
        
        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.x + p2.x, p1.y + p2.y);
        }
        
        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.x - p2.x, p1.y - p2.y);
        }
        
        public static bool operator==(Point mine, Point other)
        {
            return mine.Equals(other);
        }
    
        public static bool operator!=(Point mine, Point other)
        {
            return !mine.Equals(other);
        }
        
        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }
}