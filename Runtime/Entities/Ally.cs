namespace Glotski
{
    public class Ally : Unit
    {
        public int Attack { get; }
        
        public Ally(int id, UnitType unitType) : base(id, unitType)
        {
            Attack = (int)unitType;
        }
    }
}