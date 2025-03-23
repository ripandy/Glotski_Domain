using System.Threading;
using Cysharp.Threading.Tasks;
using Glotski.Interfaces;

namespace Glotski.GameStates
{
    public class CommanderActionState : IGameState
    {
        private readonly Commander commander;
        private readonly Stage stage;
        private readonly IMoveDirectionInputProvider moveDirectionInputProvider;
        private readonly IStagePresenter stagePresenter;

        public GameStateEnum Id => GameStateEnum.CommanderAction;
        
        public CommanderActionState(
            Commander commander,
            Stage stage,
            IMoveDirectionInputProvider moveDirectionInputProvider,
            IStagePresenter stagePresenter)
        {
            this.commander = commander;
            this.stage = stage;
            this.moveDirectionInputProvider = moveDirectionInputProvider;
            this.stagePresenter = stagePresenter;
        }
        
        public async UniTask<GameStateEnum> Running(CancellationToken cancellationToken = default)
        {
            var direction = await moveDirectionInputProvider.WaitForMoveDirection(cancellationToken);
            var moveTarget = GetMoveTarget(direction);
            commander.Target = moveTarget;

            if (stage.WouldExecuteBattle(moveTarget))
                return GameStateEnum.BattleAction;

            if (!stage.IsMovable(moveTarget))
                return GameStateEnum.CommanderAction;
            
            var movedUnit = stage[moveTarget];
            stage.Move(moveTarget);
            
            await ShowMovedUnits(movedUnit, cancellationToken);
            
            return GameStateEnum.CommanderAction;
        }

        private Point GetMoveTarget(Direction direction)
        {
            var delta = direction switch
            {
                Direction.Up => new Point(0, -1),
                Direction.Down => new Point(0, 1),
                Direction.Left => new Point(-1, 0),
                Direction.Right => new Point(1, 0),
                _ => new Point()
            };

            var startTile = stage.GetTile(commander);
            return startTile + delta;
        }

        private UniTask ShowMovedUnits(Unit movedUnit, CancellationToken cancellationToken)
        {
            var unitTasks = new[]
            {
                stagePresenter.MoveUnitAsync(commander, stage.GetTile(commander), cancellationToken),
                stagePresenter.MoveUnitAsync(movedUnit, stage.GetTile(movedUnit), cancellationToken),
                stagePresenter.FlipUnitAsync(movedUnit, cancellationToken: cancellationToken)
            };

            return UniTask.WhenAll(unitTasks);
        }
    }
}