using System.Threading;
using System.Threading.Tasks;
using Glotski.GameStates;
using Glotski.Tests.DummyClasses;
using NUnit.Framework;

namespace Glotski.Tests
{
    public class CommanderActionStateTests
    {
        private readonly Commander commander = new();
        private readonly DummyMoveDirectionInputProvider dummyMoveDirectionInputProvider = new();
        private readonly DummyStagePresenter dummyStagePresenter = new();

        [Test]
        public async Task CommanderActionState_Running_ShouldRunProperly()
        {
            var unitIds = new[]
            {
                2, 2, 2, 2,
                3, 5, 4, 2,
                3, 3, 4, 4,
                3, 1, 5, 5
            };
            var stage = StageFactory.Create(unitIds, width: 4, commander);
            var commanderActionState = new CommanderActionState(commander, stage, dummyMoveDirectionInputProvider, dummyStagePresenter);
            using var cts = new CancellationTokenSource();
            
            stage.PopulateInvolvedAllies();

            var startTile = stage.GetTile(commander);
            dummyMoveDirectionInputProvider.Direction = Direction.Right;
            var nextState = await commanderActionState.Running(cts.Token);
            Assert.AreEqual(GameStateEnum.CommanderAction, nextState, "Should return CommanderAction state due to unable to move.");
            Assert.AreEqual(startTile, stage.GetTile(commander), "Commander should not be moved due to invalid move direction.");
            
            dummyMoveDirectionInputProvider.Direction = Direction.Up;
            nextState = await commanderActionState.Running(cts.Token);
            var endTile = startTile + new Point(0, -1);
            Assert.AreEqual(GameStateEnum.CommanderAction, nextState, "Should return CommanderAction state due move completed.");
            Assert.AreEqual(stage.GetTile(commander), endTile, "Commander should be moved to the destination.");
            
            dummyMoveDirectionInputProvider.Direction = Direction.Up;
            nextState = await commanderActionState.Running(cts.Token);
            var targetTile = endTile + new Point(0, -1);
            Assert.AreEqual(GameStateEnum.BattleAction, nextState, "Should return BattleAction state due to enemy encounter.");
            Assert.AreEqual(stage.GetTile(commander), endTile, "Commander should not be moved due to enemy encounter.");
            Assert.AreEqual(commander.Target, targetTile, "Commander target should be set to the enemy location.");
        }
    }
}