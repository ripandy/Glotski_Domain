using System.Threading;
using Cysharp.Threading.Tasks;
using Glotski.Interfaces;

namespace Glotski.Tests.DummyClasses
{
    public class DummyMoveDirectionInputProvider : IMoveDirectionInputProvider
    {
        public Direction Direction { get; set; }
        
        public async UniTask<Direction> WaitForMoveDirection(CancellationToken cancellationToken = default)
        {
            await UniTask.Delay(100, cancellationToken: cancellationToken);
            return Direction;
        }
    }
}