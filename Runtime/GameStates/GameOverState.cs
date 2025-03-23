using System.Threading;
using Cysharp.Threading.Tasks;
using Glotski.Interfaces;

namespace Glotski.GameStates
{
    public class GameOverState : IGameState
    {
        private readonly IGameOverPresenter gameOverPresenter;
        
        public GameStateEnum Id => GameStateEnum.GameOver;
        
        public GameOverState(IGameOverPresenter gameOverPresenter)
        {
            this.gameOverPresenter = gameOverPresenter;
        }
        
        public async UniTask<GameStateEnum> Running(CancellationToken cancellationToken = default)
        {
            var isReplay = await gameOverPresenter.Show(cancellationToken);
            return isReplay ? GameStateEnum.Intro : GameStateEnum.None;
        }
    }
}