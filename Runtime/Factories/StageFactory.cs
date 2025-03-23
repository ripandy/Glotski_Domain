using System.Linq;

namespace Glotski
{
    public static class StageFactory
    {
        public static Stage Create(int[] unitTypeIds, int width, Commander commander = null)
        {
            var increment = 1;
            var generatedUnits = unitTypeIds.Select((typeId, index) =>
            {
                if (typeId == 1)
                    increment = 0;
                var id = index + increment;
                Unit unit = typeId switch
                {
                    1 => commander ??= new Commander(),
                    2 => new Ally(id, UnitType.Ally1),
                    3 => new Ally(id, UnitType.Ally2),
                    4 => new Ally(id, UnitType.Ally3),
                    5 => new Enemy(id),
                    6 => new Enemy(id, isFaceUp: false),
                    _ => null
                };
                return unit;
            });
            
            return new Stage(generatedUnits, width);
        }
        
        public static Stage Create(int[] unitTypeIds, int width, out Commander commander)
        {
            var increment = 1;
            var generatedUnits = unitTypeIds.Select((typeId, index) =>
            {
                if (typeId == 1)
                    increment = 0;
                var id = index + increment;
                Unit unit = typeId switch
                {
                    1 => new Commander(),
                    2 => new Ally(id, UnitType.Ally1),
                    3 => new Ally(id, UnitType.Ally2),
                    4 => new Ally(id, UnitType.Ally3),
                    5 => new Enemy(id),
                    6 => new Enemy(id, isFaceUp: false),
                    _ => null
                };
                return unit;
            }).ToArray();
            
            commander = generatedUnits.OfType<Commander>().First();
            
            return new Stage(generatedUnits, width);
        }
    }
}