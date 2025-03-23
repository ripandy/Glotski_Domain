using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Glotski.Interfaces;

namespace Glotski.GameStates
{
    public class BattleActionState : IGameState
    {
        private readonly Commander commander;
        private readonly Stage stage;
        private readonly IStagePresenter stagePresenter;

        public GameStateEnum Id => GameStateEnum.BattleAction;
        
        public BattleActionState(
            Commander commander,
            Stage stage,
            IStagePresenter stagePresenter)
        {
            this.commander = commander;
            this.stage = stage;
            this.stagePresenter = stagePresenter;
        }
        
        public async UniTask<GameStateEnum> Running(CancellationToken cancellationToken = default)
        {
            var enemy = stage[commander.Target] as Enemy;
            var involvedAllies = stage.GetInvolvedAllies(enemy).ToArray();
            
            stage.ExecuteBattle(enemy);
            
            await ShowBattleAction(enemy, involvedAllies, cancellationToken);
            var flipTasks = FlipBattledUnits(enemy, involvedAllies, cancellationToken).ToArray();
            
            stage.Move(commander.Target);
            
            var moveTasks = ShowMovedUnits(enemy, cancellationToken).Concat(flipTasks);
            
            await UniTask.WhenAll(moveTasks);

            return stage.IsCleared ? GameStateEnum.GameOver : GameStateEnum.CommanderAction;
        }

        private IEnumerable<UniTask> ShowBattleAction(Enemy enemy, IReadOnlyCollection<Ally> involvedAllies, CancellationToken cancellationToken)
        {
            var involvedUnits = involvedAllies
                .Cast<Unit>()
                .Append(commander);
            
            return involvedUnits.Select(unit =>
            {
                var comboDepth = stage.GetDistance(unit, enemy);
                return stagePresenter.ShowAttackAsync(unit, comboDepth, cancellationToken);
            });
        }

        private IEnumerable<UniTask> FlipBattledUnits(Enemy enemy, IReadOnlyCollection<Ally> involvedAllies, CancellationToken cancellationToken)
        {
            var involvedUnits = involvedAllies
                .Cast<Unit>()
                .Append(enemy);
            
            return involvedUnits.Select(unit =>
            {
                var comboDepth = stage.GetDistance(unit, enemy);
                return stagePresenter.FlipUnitAsync(unit, comboDepth, cancellationToken);
            });
        }

        private IEnumerable<UniTask> ShowMovedUnits(Unit movedUnit, CancellationToken cancellationToken)
        {
            return new[]
            {
                stagePresenter.MoveUnitAsync(commander, stage.GetTile(commander), cancellationToken),
                stagePresenter.MoveUnitAsync(movedUnit, stage.GetTile(movedUnit), cancellationToken)
            };
        }
    }
}