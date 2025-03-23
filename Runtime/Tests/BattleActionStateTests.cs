using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glotski.GameStates;
using Glotski.Tests.DummyClasses;
using NUnit.Framework;

namespace Glotski.Tests
{
    public class BattleActionStateTests
    {
        private readonly Commander commander = new(target: new Point(1, 1));
        private readonly DummyStagePresenter dummyStagePresenter = new();

        private static IEnumerable<TestCaseData> GenerateTestCaseData()
        {
            int[][] testUnitIds = 
            {
                new[]
                {
                    2, 2, 2, 2,
                    3, 5, 4, 2,
                    3, 1, 4, 4,
                    3, 3, 5, 5
                },
                new[]
                {
                    2, 2, 2, 2,
                    3, 5, 4, 2,
                    3, 1, 4, 4,
                    3, 3, 6, 6
                }
            };
            
            yield return new TestCaseData(testUnitIds[0], GameStateEnum.CommanderAction);
            yield return new TestCaseData(testUnitIds[1], GameStateEnum.GameOver);
        }
        
        [TestCaseSource(nameof(GenerateTestCaseData))]
        public async Task CommanderActionState_Running_ShouldRunProperly(int[] unitIds, GameStateEnum expectedNextState)
        {
            var stage = StageFactory.Create(unitIds, width: 4, commander);
            var battleActionState = new BattleActionState(commander, stage, dummyStagePresenter);
            var cts = new CancellationTokenSource();
            
            stage.PopulateInvolvedAllies();

            var targetEnemy = (Enemy)stage[commander.Target];
            var nextState = await battleActionState.Running(cts.Token);
            var involvedAllies = stage.GetInvolvedAllies(targetEnemy);
            var allTired = involvedAllies.All(ally => !ally.IsFaceUp);
            Assert.AreEqual(expectedNextState, nextState, "Should return expected next state.");
            Assert.IsTrue(!targetEnemy.IsFaceUp, "Enemy should be defeated after battle action.");
            Assert.IsTrue(allTired, "All involved allies should be tired after battle action.");
        }
    }
}