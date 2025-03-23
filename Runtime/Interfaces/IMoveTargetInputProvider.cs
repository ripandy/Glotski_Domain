using System.Threading;
using Cysharp.Threading.Tasks;

namespace Glotski.Interfaces
{
    public interface IMoveDirectionInputProvider
    {
        UniTask<Direction> WaitForMoveDirection(CancellationToken cancellationToken = default);
    }
}