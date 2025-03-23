namespace Glotski
{
    public class Commander : Unit
    {
        public Point Target { get; internal set; }
        
        public Commander() : base(0, UnitType.Commander)
        {
        }
        
        public Commander(Point target) : base(0, UnitType.Commander)
        {
            Target = target;
        }
    }
}