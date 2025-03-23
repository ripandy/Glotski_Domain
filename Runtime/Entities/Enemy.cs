using UnityEngine;

namespace Glotski
{
    public class Enemy : Unit
    {
        public int HP { get; private set; }
        public int SubType { get; internal set; }

        public Enemy(int id, bool isFaceUp = true)
            : base(id, UnitType.Enemy)
        {
            IsFaceUp = isFaceUp;
        }

        public void SetHP(int minHP, int maxHP)
        {
            HP = Random.Range(minHP, maxHP + 1);
        }
    }
}