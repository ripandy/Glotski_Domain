using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Glotski
{
    public class Stage
    {
        private readonly List<Unit> units;
        private readonly Dictionary<Unit, HashSet<Unit>> involvedAllies = new();

        public int Width { get; }
        public int Height { get; }
        private int UnitCount => Width * Height;

        private readonly int minTotalAttack;
        private readonly int maxTotalAttack;
        private readonly int skillThreshold;
        private readonly int enemyCount;
        
        internal IEnumerable<Unit> Units => units;
        private Commander Commander { get; }
        private IEnumerable<Enemy> Enemies => units.OfType<Enemy>();
        public IEnumerable<Ally> GetInvolvedAllies(Unit unit) => involvedAllies[unit].OfType<Ally>();

        public Stage(IEnumerable<Unit> units, int width)
        {
            this.units = units.ToList();

            Width = width;
            Height = this.units.Count / width;
            
            Commander = this.units.OfType<Commander>().First();
            
            var allies = this.units.OfType<Ally>().ToArray();
            var minAtk = allies.Min(ally => ally.Attack);
            minTotalAttack = allies.Where(ally => ally.Attack == minAtk).Sum(ally => ally.Attack);
            maxTotalAttack = allies.Sum(ally => ally.Attack);
            skillThreshold = maxTotalAttack * 2 / 3;
            enemyCount = this.units.OfType<Enemy>().Count();
            
            involvedAllies.Add(Commander, new HashSet<Unit>()); // Commander needs to be added for Move Enemy Skills.
            foreach (var enemy in Enemies)
            {
                involvedAllies.Add(enemy, new HashSet<Unit>());
            }
        }

        private Unit this[int tile]
        {
            get => units[tile];
            set => units[tile] = value;
        }

        private Unit this[int x, int y] => this[Width * y + x];
        public Unit this[Point point] => this[point.x, point.y];

        private int GetIndex(Unit unit) => units.IndexOf(unit);
        public Point GetTile(Unit unit)
        {
            var index = GetIndex(unit);
            return new Point(index % Width, index / Width);
        }
        
        public int GetDistance(Unit from, Unit to)
        {
            var origin = GetTile(from);
            var target = GetTile(to);
            return Mathf.Abs(target.x - origin.x) + Mathf.Abs(target.y - origin.y);
        }

        public void Initialize()
        {
            var counter = 0;
            foreach (var unit in units)
            {
                unit.Initialize();
                if (unit is not Enemy enemy) continue;
                InitializeEnemy(enemy, counter++);
            }
            
            Shuffle();
            PopulateInvolvedAllies();

            void InitializeEnemy(Enemy enemy, int tier)
            {
                var splitValue = (maxTotalAttack - minTotalAttack) / enemyCount;
                var minHP = minTotalAttack + splitValue * tier;
                var maxHP = minHP + splitValue;
                enemy.SetHP(minHP, maxHP);
                enemy.SubType = enemy.HP - minTotalAttack;
            }
        }
        
        private void Swap(int tile1, int tile2)
        {
            (this[tile1], this[tile2]) = (this[tile2], this[tile1]);
        }

        private void Shuffle()
        {
            for (var i = UnitCount - 1; i > 0; i--)
            {
                Swap(i, Random.Range(0, i - 1));
            }
        }
        
        private bool IsTileValid(Point tile)
        {
            return tile.x >= 0 && tile.x < Width &&
                   tile.y >= 0 && tile.y < Height;
        }

        public bool IsMovable(Point targetTile)
        {
            if (!IsTileValid(targetTile) || targetTile == GetTile(Commander)) return false;
            
            var totalAttack = GetTotalAttack(Commander);
            var isSkillUsable = totalAttack >= skillThreshold;
            return this[targetTile] is not Enemy enemy || isSkillUsable || !enemy.IsFaceUp;
        }

        public void Move(Point point)
        {
            var index = GetIndex(Commander);
            var target = this[point];
            Swap(index, GetIndex(target));
            if (target is Ally && !target.IsFaceUp)
            {
                target.Flip();
            }
            PopulateInvolvedAllies();
        }

        public void PopulateInvolvedAllies()
        {
            foreach (var (unit, allies) in involvedAllies)
            {
                allies.Clear();
                if (unit is not (Glotski.Commander or Enemy { IsFaceUp: true })) continue;
                GetSurroundingAllies(unit, allies);
            }
        }

        private void GetSurroundingAllies(Unit target, HashSet<Unit> allies)
        {
            if (target is Ally && !allies.Add(target)) return;
            
            var index = GetIndex(target);
            var point = new Point(index % Width, index / Width);
            
            var left = point.x - 1 >= 0 ? this[point.x - 1, point.y] : null;
            var right = point.x + 1 < Width ? this[point.x + 1, point.y] : null;
            var up = point.y - 1 >= 0 ? this[point.x, point.y - 1] : null;
            var down = point.y + 1 < Height ? this[point.x, point.y + 1] : null;
    
            if (ShouldProceed(left)) GetSurroundingAllies(left, allies);
            if (ShouldProceed(right)) GetSurroundingAllies(right, allies);
            if (ShouldProceed(up)) GetSurroundingAllies(up, allies);
            if (ShouldProceed(down)) GetSurroundingAllies(down, allies);

            bool ShouldProceed(Unit nextUnit)
            {
                return nextUnit is { IsFaceUp: true } &&
                       (nextUnit is Ally && target is not Ally ||
                        IsAllyOfEqualAttackPower(nextUnit, target));
            }

            bool IsAllyOfEqualAttackPower(Unit unit1, Unit unit2)
            {
                return unit1 is Ally ally1 &&
                       unit2 is Ally ally2 &&
                       ally1.Attack == ally2.Attack;
            }
        }

        public int GetTotalAttack(Unit target)
        {
            return involvedAllies.TryGetValue(target, out var allies)
                ? allies.OfType<Ally>().Sum(ally => ally.Attack)
                : 0;
        }
        
        public bool WouldExecuteBattle(Point point)
        {
            if (!IsTileValid(point) || this[point] is not Enemy enemy)
            {
                return false;
            }
            
            var totalAttack = GetTotalAttack(enemy);
            Debug.Log($"[{GetType().Name}] Trying to attack enemy ({enemy.HP}) with total attack: {totalAttack}");
            return enemy.IsFaceUp && totalAttack >= enemy.HP;
        }
        
        public bool IsCleared => Enemies.All(enemy => !enemy.IsFaceUp);

        public void ExecuteBattle(Enemy enemy)
        {
            var involvedUnits = involvedAllies[enemy].Append(enemy);
            foreach (var unit in involvedUnits)
            {
                unit.Flip();
            }
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine();
            for (var i = 0; i < UnitCount; i++)
            {
                sb.Append($"{(int)units[i].Type}, ");
                if (i % Width == Width - 1)
                {
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }
    }
}