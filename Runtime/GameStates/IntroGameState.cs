using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Glotski.Interfaces;

namespace Glotski.GameStates
{
    public class IntroGameState : IGameState
    {
        private readonly Stage stage;
        private readonly IIntroPresenter introPresenter;
        private readonly IStagePresenter stagePresenter;
        
        public GameStateEnum Id => GameStateEnum.Intro;
        
        public IntroGameState(Stage stage, IIntroPresenter introPresenter, IStagePresenter stagePresenter)
        {
            this.stage = stage;
            this.introPresenter = introPresenter;
            this.stagePresenter = stagePresenter;
        }
        
        public async UniTask<GameStateEnum> Running(CancellationToken cancellationToken = default)
        {
            var showIntroTask = introPresenter.Show(cancellationToken);
            
            stage.Initialize();

            await showIntroTask;
            await ShowUnitInitialization(cancellationToken);
            
            return GameStateEnum.CommanderAction;
        }

        private async UniTask ShowUnitInitialization(CancellationToken cancellationToken)
        {
            var units = stage.Units.ToArray();
            var showUnitTask = stagePresenter.InitializeStageAsync(units, stage.Width, stage.Height, cancellationToken);
            var moveUnitTasks = units.Select(unit =>
            {
                var tile = stage.GetTile(unit);
                return stagePresenter.MoveUnitAsync(unit, tile, cancellationToken);
            });
            
            await UniTask.WhenAll(moveUnitTasks.Append(showUnitTask));
        }
    }
}