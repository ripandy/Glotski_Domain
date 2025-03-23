using System.Threading;
using Cysharp.Threading.Tasks;

namespace Glotski.GameStates
{
    public interface IGameState
    {
        GameStateEnum Id { get; }
        UniTask<GameStateEnum> Running(CancellationToken cancellationToken = default);
    }
}