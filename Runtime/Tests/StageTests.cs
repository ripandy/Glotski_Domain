using System.Linq;
using NUnit.Framework;

namespace Glotski.Tests
{
    public class StageTests
    {
        private readonly Commander commander = new();
        
        [Test]
        public void Stage_Move_MovesUnitCorrectly()
        {
            var unitIds = new[]
            {
                2, 2, 2, 2,
                4, 3, 5, 2,
                4, 3, 5, 4,
                3, 1, 5, 3
            };
            var stage = StageFactory.Create(unitIds, width: 4, commander);
            stage.PopulateInvolvedAllies();
            
            var start = stage.GetTile(commander);
            var destination = new Point(2, 3);
            var movable = stage.IsMovable(destination);
            Assert.IsFalse(movable, $"Should not be movable to Enemy location at destination {destination} from {start}.");
            
            destination = new Point(1, 4);
            movable = stage.IsMovable(destination);
            Assert.IsFalse(movable, $"Should not be movable to the destination {destination} from {start} due to out of bound.");

            destination = new Point(1, 2);
            movable = stage.IsMovable(destination);
            Assert.IsTrue(movable, $"Should be movable to the destination {destination} from {start}.");
            
            var unit = stage[destination];
            stage.Move(destination);
            Assert.AreEqual(unit, stage[start], "Ally unit should be moved to the start.");
            Assert.AreEqual(commander, stage[destination], "Commander should be moved to the destination.");
            
            start = stage.GetTile(commander);
            destination = new Point(1, 1);
            movable = stage.IsMovable(destination);
            Assert.IsTrue(movable, $"Should be movable to the destination {destination} from {start}.");
            
            unit = stage[destination];
            stage.Move(destination);
            Assert.AreEqual(unit, stage[start], "Ally unit should be moved to the start.");
            Assert.AreEqual(commander, stage[destination], "Commander should be moved to the destination.");
            
            start = stage.GetTile(commander);
            destination = new Point(2, 1);
            movable = stage.IsMovable(destination);
            Assert.IsTrue(movable, $"Should be movable to Enemy location at the destination {destination} from {start} due Skill Activation.");
            
            var enemy = stage[destination];
            stage.Move(destination);
            Assert.AreEqual(enemy, stage[start], "Enemy unit should be moved to the start.");
            Assert.AreEqual(commander, stage[destination], "Commander should be moved to the destination.");
            Assert.IsTrue(enemy is Enemy, "Enemy unit is of Type Enemy.");
        }

        [Test]
        public void Stage_ExecuteBattle_ShouldExecuteProperly()
        {
            var unitIds = new[]
            {
                2, 2, 2, 2,
                2, 1, 5, 4,
                5, 3, 3, 4,
                3, 3, 5, 4
            };
            var stage = StageFactory.Create(unitIds, width: 4, commander);
            stage.PopulateInvolvedAllies();
            
            var destination = new Point(2, 1);
            var wouldBattle = stage.WouldExecuteBattle(destination);
            Assert.IsTrue(wouldBattle, $"Should execute battle at the destination {destination}.");
            
            var enemy = (Enemy)stage[destination];
            var totalAttack = stage.GetTotalAttack(enemy);
            Assert.GreaterOrEqual(totalAttack, enemy.HP, $"Total attack of {totalAttack} should be greater or equal to Enemy Max HP of {enemy.HP}.");
            
            var involvedAllies = stage.GetInvolvedAllies(enemy);
            stage.ExecuteBattle(enemy);
            Assert.IsFalse(enemy.IsFaceUp, "Enemy should be defeated and flipped face down.");
            Assert.IsTrue(involvedAllies.All(ally => !ally.IsFaceUp), "All involved allies should be flipped face down.");
            
            var start = stage.GetTile(commander);
            destination = new Point(2, 1);
            var isMovable = stage.IsMovable(destination);
            Assert.IsTrue(isMovable, $"Should be movable to defeated Enemy location at the destination {destination} from {start}.");
            
            stage.Move(destination);
            Assert.AreEqual(enemy, stage[start], "Defeated Enemy unit should be moved to the start.");
            Assert.AreEqual(commander, stage[destination], "Commander should be moved to the destination.");
            Assert.IsFalse(enemy.IsFaceUp, "Defeated Enemy should be flipped face down.");

            start = stage.GetTile(commander);
            destination = new Point(3, 1);
            isMovable = stage.IsMovable(destination);
            Assert.IsTrue(isMovable, $"Should be movable to Ally location at the destination {destination} from {start}.");
            
            var unit = stage[destination];
            Assert.IsTrue(unit is Ally { IsFaceUp: false }, "Unit at destination should be of Type Ally and is Face Down.");
            stage.Move(destination);
            Assert.AreEqual(unit, stage[start], "Tired Ally unit should be moved to the start.");
            Assert.AreEqual(commander, stage[destination], "Commander should be moved to the destination.");
            Assert.IsTrue(unit.IsFaceUp, "Tired Ally should be recovered by Commander and flipped face up.");
        }
    }
}