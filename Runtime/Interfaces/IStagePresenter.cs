using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Glotski.Interfaces
{
    public interface IStagePresenter
    {
        UniTask InitializeStageAsync(IEnumerable<Unit> units, int stageWidth, int stageHeight, CancellationToken cancellationToken = default);
        UniTask MoveUnitAsync(Unit unit, Point tile, CancellationToken cancellationToken = default);
        UniTask FlipUnitAsync(Unit unit, int depth = 0, CancellationToken cancellationToken = default);
        UniTask ShowAttackAsync(Unit actor, int comboDepth, CancellationToken cancellationToken = default);
    }
}