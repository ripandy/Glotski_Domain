namespace Glotski
{
    public enum UnitType
    {
        None = 0,
        Commander,
        Ally1,
        Ally2,
        Ally3,
        Enemy
    }
    
    public class Unit
    {
        public int Id { get; }
        public UnitType Type { get; }
        public bool IsFaceUp { get; internal set; } = true;

        protected Unit(int id, UnitType type)
        {
            Id = id;
            Type = type;
        }

        internal void Initialize()
        {
            IsFaceUp = true;
        }

        internal void Flip()
        {
            IsFaceUp = !IsFaceUp;
        }
    }
}